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
        public Task<int> AddLocation(NewLocationDto newLocationDto);
        public Task<bool> RemoveLocation(string name);

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
        public async Task<int> AddLocation(NewLocationDto newLocationDto)
        {
            var newLocation = _mapper.Map<Location>(newLocationDto);
            await _locationRepository.AddLocation(newLocation);
            return newLocation.Id;
        }
        //Remove location, throws exception if there is no location with given name or if there are desks in location
        public async Task<bool> RemoveLocation(string name)
        {
            var locationToRemove = await _locationRepository.GetByName(name);
            if (locationToRemove == null)
            {
                throw new LocationNotFoundException("There is no location with given name");
            }else if(locationToRemove.Desks != null)
            {
                throw new LocationNotEmptyException("There are desks in location you want to remove");
            }
            await _locationRepository.DeleteLocation(locationToRemove);
            return true;
        }
    }
}
