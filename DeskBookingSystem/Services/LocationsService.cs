using AutoMapper;
using DeskBookingSystem.Entities;
using DeskBookingSystem.Exceptions;
using DeskBookingSystem.Models;
using DeskBookingSystem.Repositories;
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
        private readonly IMapper _mapper;
        private readonly ILocationRepository _locationRepository;

        public LocationsService(BookingSystemDbContext dbContext, IMapper mapper, ILocationRepository locationRepository)
        {
            _mapper = mapper;
            _locationRepository = locationRepository;
        }
        //Add Location
        public int AddLocation(NewLocationDto newLocationDto)
        {
            var newLocation = _mapper.Map<Location>(newLocationDto);
            _locationRepository.AddLocation(newLocation);
            return newLocation.Id;
        }
        //Remove location, throws exception if there is no location with given name or if there are desks in location
        public bool RemoveLocation(string name)
        {
            var locationToRemove = _locationRepository.GetByName(name);
            if (locationToRemove == null)
            {
                throw new LocationNotFoundException("There is no location with given name");
            }else if(locationToRemove.Desks != null)
            {
                throw new LocationNotEmptyException("There are desks in location you want to remove");
            }
            _locationRepository.DeleteLocation(locationToRemove);
            return true;
        }
    }
}
