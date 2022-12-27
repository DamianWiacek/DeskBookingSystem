using AutoMapper;
using DeskBookingSystem.Entities;
using DeskBookingSystem.Exceptions;
using DeskBookingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace DeskBookingSystem.Services
{
    public interface IDesksService
    {
        public List<DeskDto> GetDesksByLocation(string location);
        public List<DeskDtoForAdmin> GetDesksByLocationForAdmin(string location);
        public int AddDesk(NewDeskDto newDeskDto);
        public bool ManageAvailability(int id, bool availability);
        public bool RemoveDesk(int id);
    }
    public class DesksService : IDesksService
    {
        private readonly BookingSystemDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IAvailabilityService _availabilityService;

        public DesksService(BookingSystemDbContext dbContext, IMapper mapper,IAvailabilityService availabilityService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _availabilityService = availabilityService;
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

        public List<DeskDtoForAdmin> GetDesksByLocationForAdmin(string location)
        {
            var desks = from desk in _dbContext.Desks
                        .Where(d=>d.Location.Name == location)
                        join reservations in _dbContext.Reservations
                        .Where(r=>r.ReservationStart <= DateTime.Now
                        && r.ReservationEnd >= DateTime.Now)
                        on desk equals reservations.Desk into gj
                        from subreservation in gj.DefaultIfEmpty()
                        select new DeskDtoForAdmin { LocationName= desk.Location.Name,
                                                     Id = desk.Id,
                                                     ReservingUserId = subreservation.UserId,
                                                     Available = desk.Available};


            return desks.ToList();
        }
            
        public List<DeskDto> GetDesksByLocation(string location)
        {
            //var desks = _dbContext.Desks
            //    .Where(l=>l.Location.Name==location)
            //    .Include(l=>l.Location)
            //    .ToList();
            //var desksDtos = _mapper.Map<List<DeskDto>>(desks);
            //return desksDtos;
            var desks = from desk in _dbContext.Desks
                        .Where(d => d.Location.Name == location)
                        join reservations in _dbContext.Reservations
                        .Where(r => r.ReservationStart <= DateTime.Now
                        && r.ReservationEnd >= DateTime.Now)
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
            var reservations = _availabilityService.DeskIsAvailableAtGivenTime(id, DateTime.Now, DateTime.Now);
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
