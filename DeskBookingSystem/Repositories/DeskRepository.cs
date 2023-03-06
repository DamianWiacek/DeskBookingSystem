using DeskBookingSystem.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeskBookingSystem.Repositories
{
    public interface IDeskRepository
    {
        public void AddDesk(Desk desk);
        public IQueryable<Desk> GetDesks();
        public bool UpdateDeskAvailability(int id, bool availability);
        public void RemoveDesk(Desk desk);
        public Desk GetDeskById(int deskId);
    }
    public class DeskRepository : IDeskRepository
    {
        private readonly BookingSystemDbContext _dbContext;

        public DeskRepository(BookingSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public void AddDesk(Desk desk)
        {
            _dbContext.Add(desk);
            _dbContext.SaveChanges();
        }
        
       
        public IQueryable<Desk> GetDesks()
        {
            return _dbContext.Desks;
        }
        public bool UpdateDeskAvailability(int id, bool availability)
        {
            var desk = _dbContext.Desks
                .FirstOrDefault(d => d.Id == id);
            desk.Available = availability;
            _dbContext.SaveChanges();
            return true;
        }
        public void RemoveDesk(Desk desk)
        {
            _dbContext.Remove(desk);
            _dbContext.SaveChanges();
        }

        public Desk GetDeskById(int deskId)
        {
            return _dbContext.Desks.FirstOrDefault(d => d.Id == deskId);
        }
    }
}