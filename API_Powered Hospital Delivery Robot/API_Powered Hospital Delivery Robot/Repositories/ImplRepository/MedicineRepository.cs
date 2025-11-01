using API_Powered_Hospital_Delivery_Robot.Models.DTOs;
using API_Powered_Hospital_Delivery_Robot.Models.Entities;
using API_Powered_Hospital_Delivery_Robot.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace API_Powered_Hospital_Delivery_Robot.Repositories.ImplRepository
{
    public class MedicineRepository : IMedicineRepository
    {
        private readonly RobotManagerContext _context;

        public MedicineRepository(RobotManagerContext context)
        {
            _context = context;
        }

        public async Task<Medicine> CreateAsync(Medicine medicine)
        {
            _context.Medicines.Add(medicine);
            await _context.SaveChangesAsync();
            return medicine;
        }

        public async Task<IEnumerable<Medicine>> GetAllAsync(ulong? categoryId = null, MedicineStatus? status = null)
        {
            var query = _context.Medicines.AsQueryable();
            if (categoryId.HasValue)
            {
                query = query.Where(m => m.CategoryId == categoryId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(m => m.Status == status.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<Medicine?> GetByCodeAsync(string medicineCode)
        {
            return await _context.Medicines.FirstOrDefaultAsync(m => m.MedicineCode == medicineCode);
        }

        public async Task<Medicine?> GetByIdAsync(ulong id)
        {
            return await _context.Medicines.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<int> RemoveExpiredAsync()
        {
            var expired = await _context.Medicines.Where(m => m.Status == MedicineStatus.Expired).ToListAsync();
            _context.Medicines.RemoveRange(expired);
            await _context.SaveChangesAsync();
            return expired.Count;
        }

        public async Task<ScanExpiredResponseDto> ScanAndFlagExpiredAsync(bool flagOnly = true)
        {
            var now = DateTime.UtcNow;
            var expiredMedicines = await _context.Medicines.Where(m => m.ExpiryDate < now && m.Status == MedicineStatus.Active).ToListAsync();

            var expiredCodes = new List<string>();
            foreach (var medicine in expiredMedicines)
            {
                medicine.Status = MedicineStatus.Expired;
                if (!flagOnly) medicine.StockQuantity = 0; // Clear stock if hard
                expiredCodes.Add(medicine.MedicineCode);
            }

            if (expiredMedicines.Any()) await _context.SaveChangesAsync();

            return new ScanExpiredResponseDto
            {
                TotalScanned = expiredMedicines.Count,
                ExpiredCount = expiredMedicines.Count,
                ExpiredCodes = expiredCodes
            };
        }

        public async Task<Medicine?> UpdateAsync(ulong id, Medicine medicine)
        {
            var existing = await _context.Medicines.FindAsync(id);
            if (existing == null)
            {
                return null;
            }

            existing.MedicineCode = medicine.MedicineCode;
            existing.Name = medicine.Name;
            existing.Unit = medicine.Unit;
            existing.Price = medicine.Price;
            existing.StockQuantity = medicine.StockQuantity;
            existing.Description = medicine.Description;
            existing.CategoryId = medicine.CategoryId;

            await _context.SaveChangesAsync();
            return existing;
        }
    }
}
