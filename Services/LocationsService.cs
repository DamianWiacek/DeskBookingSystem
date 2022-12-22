using AutoMapper;
using DeskBookingSystem.Entities;
using DeskBookingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeskBookingSystem.Services
{
    public interface IlocationService
    {
        public int AddLocation(NewLocationDto newLocationDto);
        public int RemoveLocation();

    }
    public class LocationsService : IlocationService
    {
        private readonly BookingSystemDbContext _dbContext;
        private readonly IMapper _mapper;

        public LocationsService(BookingSystemDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public int AddLocation(NewLocationDto newLocationDto)
        {
            var newLocation = _mapper.Map<Location>(newLocationDto);
            _dbContext.Locations.Add(newLocation);
            _dbContext.SaveChanges();
            return newLocation.Id;
        }

        public int RemoveLocation()
        {
            throw new NotImplementedException();
        }
    }
}
