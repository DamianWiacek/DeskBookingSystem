using DeskBookingSystem.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeskBookingSystem.Repositories
{
    public interface ILocationRepository
    {
        public IQueryable<Location> GetLocations();
        public void AddLocation(Location location);
        public Location GetByName(string name);
        public void DeleteLocation(Location location); 
    }
    public class LocationRepository : ILocationRepository
    {
        private readonly BookingSystemDbContext _dbContext;

        public LocationRepository(BookingSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddLocation(Location location)
        {
            _dbContext.Locations.Add(location);
            _dbContext.SaveChanges();
        }

        public void DeleteLocation(Location location)
        {
            _dbContext.Remove(location);
            _dbContext.SaveChanges();
        }

        public Location GetByName(string name)
        {
            return _dbContext.Locations.FirstOrDefault(l => l.Name == name);
        }

        public IQueryable<Location> GetLocations()
        {
            return _dbContext.Locations;
        }
    }
}
