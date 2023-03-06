using DeskBookingSystem.Entities;
using DeskBookingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace DeskBookingSystem.Repositories
{
    public interface IUserRepository
    {
        public Task AddUser(User user);
        public Task<User> GetUserByEmail(string email);
    }
    public class UserRepository : IUserRepository
    {
        private readonly BookingSystemDbContext _dbContext;

        public UserRepository(BookingSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddUser(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<User> GetUserByEmail(string email)
        {
            return await _dbContext.Users
            .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Email == email);
        }
    }


}
