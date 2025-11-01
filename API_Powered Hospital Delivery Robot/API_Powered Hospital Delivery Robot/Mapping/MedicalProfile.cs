using API_Powered_Hospital_Delivery_Robot.Models.DTOs;
using API_Powered_Hospital_Delivery_Robot.Models.Entities;
using AutoMapper;

namespace API_Powered_Hospital_Delivery_Robot.Mapping
{
    public class MedicalProfile : Profile
    {
        public MedicalProfile()
        {
            // ✅ Patient mapping với xử lý DateTime? -> DateOnly?
            CreateMap<PatientDto, Patient>()
                .ForMember(dest => dest.Dob,
                    opt => opt.MapFrom(src =>
                        src.Dob.HasValue ? DateOnly.FromDateTime(src.Dob.Value) : (DateOnly?)null));

            CreateMap<Patient, PatientResponseDto>()
                .ForMember(dest => dest.RoomName,
                    opt => opt.MapFrom(src => src.Room != null ? src.Room.RoomName : null))
                .ForMember(dest => dest.Dob,
                    opt => opt.MapFrom(src =>
                        src.Dob.HasValue ? src.Dob.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null));

            // DrugCategory
            CreateMap<DrugCategoryDto, DrugCategory>();
            CreateMap<DrugCategory, DrugCategoryResponseDto>();

            // Medicine
            CreateMap<MedicineDto, Medicine>()
                .ForMember(dest => dest.ExpiryDate, opt => opt.MapFrom(src => src.ExpiryDate))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

            CreateMap<Medicine, MedicineResponseDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
                .ForMember(dest => dest.ExpiryDate, opt => opt.MapFrom(src => src.ExpiryDate))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

            // Prescription
            CreateMap<PrescriptionDto, Prescription>();
            CreateMap<Prescription, PrescriptionResponseDto>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient != null ? src.Patient.FullName : null))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.Users != null ? src.Users.Email : null))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.PrescriptionItems));

            // Room
            CreateMap<RoomDto, Room>();
            CreateMap<Room, RoomResponseDto>();
        }
    }
}
