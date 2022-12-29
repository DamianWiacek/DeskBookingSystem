using AutoMapper;
using DeskBookingSystem.Entities;
using DeskBookingSystem.Exceptions;
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
        public bool RemoveLocation(string name);

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
        //Add Location
        public int AddLocation(NewLocationDto newLocationDto)
        {
            var newLocation = _mapper.Map<Location>(newLocationDto);
            _dbContext.Locations.Add(newLocation);
            _dbContext.SaveChanges();
            return newLocation.Id;
        }
        //Remove location, throws exception if there is no location with given name or if there are desks in location
        public bool RemoveLocation(string name)
        {
            var locationToRemove = _dbContext.Locations
                .FirstOrDefault(l => l.Name == name);
            if (locationToRemove == null)
            {
                throw new LocationNotFoundException("There is no location with given name");
            }else if(locationToRemove.Desks != null)
            {
                throw new LocationNotEmptyException("There are desks in location you want to remove");
            }
            _dbContext.Remove(locationToRemove);
            _dbContext.SaveChanges();
            return true;
        }
    }
}
