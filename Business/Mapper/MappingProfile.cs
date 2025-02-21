﻿using AutoMapper;
using DataAccess.Entities;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<VirtualAppointmentDTO, VirtualAppointment>();
            CreateMap<VirtualAppointment, VirtualAppointmentDTO>();
        }
    }
}
