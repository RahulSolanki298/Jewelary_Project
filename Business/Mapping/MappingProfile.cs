using AutoMapper;
using DataAccess.Entities;
using Models;

namespace Business.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<VirtualAppointmentDTO, VirtualAppointment>().ReverseMap();
        }
    }
}
