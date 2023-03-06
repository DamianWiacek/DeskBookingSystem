using DeskBookingSystem.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeskBookingSystem.Repositories
{
    public interface ILocationRepository
    {
        public Task<List<Location>> GetLocations();
        public Task AddLocation(Location location);
        public Task<Location> GetByName(string name);
        public Task DeleteLocation(Location location); 
    }
    public class LocationRepository : ILocationRepository
    {
        private readonly BookingSystemDbContext _dbContext;

        public LocationRepository(BookingSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddLocation(Location location)
        {
            await _dbContext.Locations.AddAsync(location);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteLocation(Location location)
        {
            _dbContext.Remove(location);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Location> GetByName(string name)
        {
            return await _dbContext.Locations.FirstOrDefaultAsync(l => l.Name == name);
        }

        public async Task<List<Location>> GetLocations()
        {
            return await _dbContext.Locations.ToListAsync();  
        }
    }
}
