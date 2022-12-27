using AutoMapper;
using DeskBookingSystem.Entities;
using DeskBookingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeskBookingSystem
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<NewLocationDto, Location>();

            CreateMap<NewDeskDto, Desk>();

            CreateMap<Desk, DeskDto>()
                .ForMember(d=>d.LocationName, m=>m.MapFrom(l=>l.Location.Name));
            CreateMap<NewReservationDto, Reservation>();
                
        }
    }
}
