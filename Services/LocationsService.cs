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
        public string RemoveLocation(string name);

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

        public string RemoveLocation(string name)
        {
            var locationToRemove = _dbContext.Locations
                .FirstOrDefault(l => l.Name == name);
            if (locationToRemove == null)
            {
                return "NotFound";
            }else if(locationToRemove.Desks != null)
            {
                return "BadRequest";
            }
            _dbContext.Remove(locationToRemove);
            _dbContext.SaveChanges();
            return "Deleted";
        }
    }
}
