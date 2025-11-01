using API_Powered_Hospital_Delivery_Robot.Models.Entities;
using API_Powered_Hospital_Delivery_Robot.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace API_Powered_Hospital_Delivery_Robot.Repositories.ImplRepository
{
    public class PatientRepository : IPatientRepository
    {
        private readonly RobotManagerContext _context;

        public PatientRepository(RobotManagerContext context)
        {
            _context = context;
        }

        public async Task<Patient> CreateAsync(Patient patient)
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            return patient;
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            return await _context.Patients.Include(p => p.Room).ToListAsync();
        }

        public async Task<Patient?> GetByCodeAsync(string patientCode)
        {
            return await _context.Patients.FirstOrDefaultAsync(p => p.PatientCode == patientCode);
        }

        public async Task<Patient?> GetByIdAsync(ulong id, bool includeRoom = false, bool includePrescriptions = false)
        {
            var query = _context.Patients.AsQueryable();
            if (includeRoom)
            {
                query = query.Include(p => p.Room);
            }

            if (includePrescriptions)
            {
                query = query.Include(p => p.Prescriptions).ThenInclude(pr => pr.PrescriptionItems);
            }

            return await query.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Patient?> UpdateAsync(ulong id, Patient patient)
        {
            var existing = await _context.Patients.FindAsync(id);
            if (existing == null)
            {
                return null;
            }

            existing.PatientCode = patient.PatientCode;
            existing.FullName = patient.FullName;
            existing.Gender = patient.Gender;
            existing.Dob = patient.Dob;
            existing.Address = patient.Address;
            existing.Phone = patient.Phone;
            existing.Department = patient.Department;
            existing.RoomNumber = patient.RoomNumber;
            existing.RoomId = patient.RoomId;

            await _context.SaveChangesAsync();
            return existing;
        }
    }
}
