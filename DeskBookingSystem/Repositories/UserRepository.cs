using DeskBookingSystem.Entities;
using DeskBookingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace DeskBookingSystem.Repositories
{
    public interface IUserRepository
    {
        public void AddUser(User user);
        public User GetUserByEmail(string email);
    }
    public class UserRepository : IUserRepository
    {
        private readonly BookingSystemDbContext _dbContext;

        public UserRepository(BookingSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void AddUser(User user)
        {
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
        }
        public User GetUserByEmail(string email)
        {
            return _dbContext.Users
            .Include(x => x.Role)
                .FirstOrDefault(x => x.Email == email);
        }
    }


}
