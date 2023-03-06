using DeskBookingSystem.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeskBookingSystem.Repositories
{
    public interface IDeskRepository
    {
        public Task AddDesk(Desk desk);
        public Task<List<Desk>> GetDesks();
        public Task<bool> UpdateDeskAvailability(int id, bool availability);
        public Task RemoveDesk(Desk desk);
        public Task<Desk> GetDeskById(int deskId);
    }
    public class DeskRepository : IDeskRepository
    {
        private readonly BookingSystemDbContext _dbContext;

        public DeskRepository(BookingSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task AddDesk(Desk desk)
        {
            await _dbContext.AddAsync(desk);
            await _dbContext.SaveChangesAsync();
        }
        
       
        public async Task<List<Desk>> GetDesks()
        {
            return await _dbContext.Desks.ToListAsync();
        }
        public async Task<bool> UpdateDeskAvailability(int id, bool availability)
        {
            var desk = await _dbContext.Desks
                .FirstOrDefaultAsync(d => d.Id == id);
            desk.Available = availability;
            _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task RemoveDesk(Desk desk)
        {
            _dbContext.Remove(desk);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Desk> GetDeskById(int deskId)
        {
            return await _dbContext.Desks.FirstOrDefaultAsync(d => d.Id == deskId);
        }
    }
}