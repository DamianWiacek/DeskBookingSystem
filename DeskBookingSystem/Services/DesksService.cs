using AutoMapper;
using DeskBookingSystem.Entities;
using DeskBookingSystem.Exceptions;
using DeskBookingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace DeskBookingSystem.Services
{
    public interface IDesksService
    {
        public List<DeskDto> GetDesksByLocation(string locationName, DateTime sinceWhen, DateTime tillWhen);
        public List<DeskDtoForAdmin> GetDesksByLocationForAdmin(string locationName, DateTime sinceWhen, DateTime tillWhen);
        public int AddDesk(NewDeskDto newDeskDto);
        public bool ManageAvailability(int id, bool availability);
        public bool RemoveDesk(int id);
    }
    public class DesksService : IDesksService
    {
        private readonly BookingSystemDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IDateValidationService _dateValidationService;

        public DesksService(BookingSystemDbContext dbContext, IMapper mapper,IDateValidationService dateValidationService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _dateValidationService = dateValidationService;
        }
        public int AddDesk(NewDeskDto newDeskDto)
        {
            var location = _dbContext.Locations
                .FirstOrDefault(l => l.Name == newDeskDto.LocationName);
            if (location == null) throw new LocationNotFoundException("There is no location with given name");
            var newDesk = _mapper.Map<Desk>(newDeskDto);
            newDesk.LocationId = location.Id;
            _dbContext.Add(newDesk);
            _dbContext.SaveChanges();
            return newDesk.Id;

        }

        public List<DeskDtoForAdmin> GetDesksByLocationForAdmin(string locationName, DateTime sinceWhen, DateTime tillWhen)
        {
            var location = _dbContext.Locations
                .FirstOrDefault(l => l.Name == locationName);
            if (location == null) throw new LocationNotFoundException("There is no location with given name");
            var desks = from desk in _dbContext.Desks
                        .Where(d=>d.Location.Name == locationName)
                        join reservations in _dbContext.Reservations
                        .Where(r=>r.ReservationStart <= sinceWhen
                        && r.ReservationEnd >= tillWhen)
                        on desk equals reservations.Desk into gj
                        from subreservation in gj.DefaultIfEmpty()
                        select new DeskDtoForAdmin { LocationName= desk.Location.Name,
                                                     Id = desk.Id,
                                                     ReservingUserId = subreservation.UserId,
                                                     Available = desk.Available};


            return desks.ToList();
        }
            
        public List<DeskDto> GetDesksByLocation(string locationName, DateTime sinceWhen, DateTime tillWhen)
        {
            var location = _dbContext.Locations
                     .FirstOrDefault(l => l.Name == locationName);
            if (location == null) throw new LocationNotFoundException("There is no location with given name");
            var desks = from desk in _dbContext.Desks
                        .Where(d => d.Location.Name == locationName)
                        join reservations in _dbContext.Reservations
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

        public bool ManageAvailability(int id, bool availability)
        {
            var desk = _dbContext.Desks
                .FirstOrDefault(d => d.Id == id);
            if (desk == null) throw new DeskNotFoundException("There is no desk with given Id");
            desk.Available = availability;
            _dbContext.SaveChanges();
            return true;
        }

        public bool RemoveDesk(int id)
        {
            var desk = _dbContext.Desks
                .FirstOrDefault(d => d.Id == id);
            if (desk == null) throw new DeskNotFoundException("There is no desk with given Id");
            var reservations = _dateValidationService.DeskIsAvailableAtGivenTime(id, DateTime.Now, DateTime.Now);
            if (reservations == false || desk.Available == false)
            {
                throw new DeskNotAvaibleException("Desk is not available, cannot remove it.");
            }
            _dbContext.Remove(desk);
            _dbContext.SaveChanges();

            return true;
        }

    }
}
