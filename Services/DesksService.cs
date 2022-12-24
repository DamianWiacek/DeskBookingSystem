using AutoMapper;
using DeskBookingSystem.Entities;
using DeskBookingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace DeskBookingSystem.Services
{
    public interface IDesksService
    {
        public List<DeskDto> GetDesks(string location);
        public List<DeskDto> GetAllDesks();
        public int AddDesk(NewDeskDto newDeskDto);
        public bool ManageAvailability(int id, bool availability);
        public bool RemoveDesk(int id);
    }
    public class DesksService : IDesksService
    {
        private readonly BookingSystemDbContext _dbContext;
        private readonly IMapper _mapper;

        public DesksService(BookingSystemDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public int AddDesk(NewDeskDto newDeskDto)
        {
            var location = _dbContext.Locations
                .FirstOrDefault(l => l.Name == newDeskDto.LocationName);
            if (location == null) return -1;
            var newDesk = _mapper.Map<Desk>(newDeskDto);
            newDesk.LocationId = location.Id;
            _dbContext.Add(newDesk);
            _dbContext.SaveChanges();
            return newDesk.Id;

        }

        public List<DeskDto> GetAllDesks()
        {
            var desks = _dbContext.Desks.Include(d => d.Location).ToList();
            var desksDtos = _mapper.Map<List<DeskDto>>(desks);
            return desksDtos;
        }

        public List<DeskDto> GetDesks(string location)
        {
            var desks = _dbContext.Desks
                .Where(l=>l.Location.Name==location)
                .Include(l=>l.Location)
                .ToList();
            var desksDtos = _mapper.Map<List<DeskDto>>(desks);
            return desksDtos;
        }

        public bool ManageAvailability(int id, bool availability)
        {
            var desk = _dbContext.Desks
                .FirstOrDefault(d => d.Id == id);
            if (desk == null) return false;
            desk.Available = availability;
            _dbContext.SaveChanges();
            return true;
        }

        public bool RemoveDesk(int id)
        {
            var desk = _dbContext.Desks
                .FirstOrDefault(d => d.Id == id);
            if (desk == null || desk.Available == false) return false;
            _dbContext.Remove(desk);
            _dbContext.SaveChanges();

            return true;
        }

    }
}
