using API_Powered_Hospital_Delivery_Robot.Models.DTOs;
using API_Powered_Hospital_Delivery_Robot.Models.Entities;
using API_Powered_Hospital_Delivery_Robot.Repositories.IRepository;
using API_Powered_Hospital_Delivery_Robot.Services.IServices;
using AutoMapper;

namespace API_Powered_Hospital_Delivery_Robot.Services.ImplServices
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _repository;
        private readonly IMapper _mapper;

        public PatientService(IPatientRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PatientResponseDto> CreateAsync(PatientDto patientDto)
        {
            var existing = await _repository.GetByCodeAsync(patientDto.PatientCode);
            if (existing != null)
            {
                throw new InvalidOperationException("Patient code already exists");
            }

            var patient = _mapper.Map<Patient>(patientDto);
            patient.CreatedAt = DateTime.UtcNow;

            var created = await _repository.CreateAsync(patient);
            return _mapper.Map<PatientResponseDto>(created);
        }

        public async Task<IEnumerable<PatientResponseDto>> GetAllAsync()
        {
            var patients = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<PatientResponseDto>>(patients);
        }

        public async Task<PatientResponseDto?> GetByIdAsync(ulong id)
        {
            var patient = await _repository.GetByIdAsync(id, includeRoom: true, includePrescriptions: true);
            return patient != null ? _mapper.Map<PatientResponseDto>(patient) : null;
        }

        public async Task<PatientResponseDto?> UpdateAsync(ulong id, PatientDto patientDto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                throw new InvalidOperationException("Patient not found");
            }

            if (patientDto.PatientCode != existing.PatientCode)
            {
                var codeExisting = await _repository.GetByCodeAsync(patientDto.PatientCode);
                if (codeExisting != null)
                {
                    throw new InvalidOperationException("Patient code already exists");
                }
            }

            var patient = _mapper.Map<Patient>(patientDto);
            patient.Id = id;

            var updated = await _repository.UpdateAsync(id, patient);
            return updated != null ? _mapper.Map<PatientResponseDto>(updated) : null;
        }
    }
}
