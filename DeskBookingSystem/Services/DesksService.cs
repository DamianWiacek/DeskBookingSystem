using AutoMapper;
using DeskBookingSystem.Entities;
using DeskBookingSystem.Exceptions;
using DeskBookingSystem.Models;
using DeskBookingSystem.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DeskBookingSystem.Services
{
    public interface IDesksService
    {
        public Task<List<DeskDto>> GetDesksByLocation(string locationName, DateTime sinceWhen, DateTime tillWhen);
        public Task<List<DeskDtoForAdmin>> GetDesksByLocationForAdmin(string locationName, DateTime sinceWhen, DateTime tillWhen);
        public Task<int> AddDesk(NewDeskDto newDeskDto);
        public Task<bool> ManageAvailability(int id, bool availability);
        public Task<bool> RemoveDesk(int id);
    }
    public class DesksService : IDesksService
    {
        private readonly IDeskRepository _deskRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IMapper _mapper;
        private readonly IDateValidationService _dateValidationService;

        public DesksService( IMapper mapper,IDateValidationService dateValidationService, IDeskRepository deskRepository, ILocationRepository locationRepository, IReservationRepository reservationRepository)
        {
            _deskRepository = deskRepository;
            _locationRepository = locationRepository;
            _reservationRepository = reservationRepository;
            _mapper = mapper;
            _dateValidationService = dateValidationService;
        }

        //Add new desk, throws exception if there is no location with given name 
        public async Task<int> AddDesk(NewDeskDto newDeskDto)
        {
            var location = await _locationRepository.GetByName(newDeskDto.LocationName);
            if (location == null) throw new LocationNotFoundException("There is no location with given name");
            var newDesk = _mapper.Map<Desk>(newDeskDto);
            newDesk.LocationId = location.Id;
            await _deskRepository.AddDesk(newDesk);
            return newDesk.Id;

        }
        //List all desks and their availability at given time, its administrator version so users will be able to
        //see if desks is reserved, id of user who is reserving it, and its availability
        public async Task<List<DeskDtoForAdmin>> GetDesksByLocationForAdmin(string locationName, DateTime sinceWhen, DateTime tillWhen)
        {
            var location = _locationRepository.GetByName(locationName);
            if (location == null) throw new LocationNotFoundException("There is no location with given name");
            //Left join to desks with given location desks where there is some reservation at given time
            //from joined reservations, LINQ map UserID to ReservingUserId in DeskDtoForAdmin
            var desks = from desk in (await _deskRepository.GetDesks())
                        .Where(d=>d.Location.Name == locationName)
                        join reservations in (await _reservationRepository.GetReservations())
                        .Where(r=>r.ReservationStart <= sinceWhen
                        && r.ReservationEnd >= tillWhen)
                        on desk equals reservations.Desk into gj
                        from subreservation in gj.DefaultIfEmpty()
                        select new DeskDtoForAdmin { LocationName= desk.Location.Name,
                                                     Id = desk.Id,
                                                     ReservingUserId =subreservation !=null ? subreservation.UserId : 0,
                                                     Available = desk.Available};


            return desks.ToList();
        }
        
        //List all desks and their availability at given time, its employee version so users will not be able to
        //see if desks is reserved, or not available for some other reason nor see who is reserving
        public async Task<List<DeskDto>> GetDesksByLocation(string locationName, DateTime sinceWhen, DateTime tillWhen)
        {
            var location = _locationRepository.GetByName(locationName);
            if (location == null) throw new LocationNotFoundException("There is no location with given name");
            //Left join to desks with given location desks where there is some reservation at given time
            //desks where is reservation or availability is set to false, will be listed as unavailable
            var desks = from desk in (await _deskRepository.GetDesks())
                        .Where(d => d.Location.Name == locationName)
                        join reservations in (await _reservationRepository.GetReservations())
                        .Where(r => r.ReservationStart <= sinceWhen
                        && r.ReservationEnd >= tillWhen)
                        on desk equals reservations.Desk into gj
                        from subreservation in gj.DefaultIfEmpty()
                        select new DeskDto
                        {
                            LocationName = desk.Location.Name,
                            Id = desk.Id,
                            Available = subreservation != null || desk.Available == false ? false : true
                        };


            return desks.ToList();

        }
        //Set availability of desk with given ID to true or false
        public async Task<bool> ManageAvailability(int id, bool availability)
        {
            var desk = await _deskRepository.GetDeskById(id);
            if (desk == null) throw new DeskNotFoundException("There is no desk with given Id");
            var update = await _deskRepository.UpdateDeskAvailability(id, availability);
            if (!update) return false;
            return true;
        }


        //Remove desk with given id, throws exception if desk id is wrong or its not available, or there is ongoing reservation
        public async Task<bool> RemoveDesk(int id)
        {
            var desk = await _deskRepository.GetDeskById(id);
            if (desk == null) throw new DeskNotFoundException("There is no desk with given Id");
            //Search for ongoing reservations
            var reservations = await _dateValidationService.DeskIsAvailableAtGivenTime(id, DateTime.Now, DateTime.Now);
            if (reservations == false || desk.Available == false)
            {
                throw new DeskNotAvaibleException("Desk is not available, cannot remove it.");
            }
            await _deskRepository.RemoveDesk(desk);

            return true;
        }

    }
}
