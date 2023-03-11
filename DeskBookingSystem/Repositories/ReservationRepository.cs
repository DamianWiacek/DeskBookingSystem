using DeskBookingSystem.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeskBookingSystem.Repositories
{
    public interface IReservationRepository
    {
        public Task<List<Reservation>> GetAll();
        public Task Add(Reservation reservation);
        public Task<Reservation> GetById(int reservationId);
        public Task SaveChanges();
        public Task<List<Reservation>> GetReservations();
    }
    public class ReservationRepository : IReservationRepository
    {
        private readonly BookingSystemDbContext _dbContext;

        public ReservationRepository(BookingSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Reservation>> GetAll()
        {
            return await _dbContext.Reservations.ToListAsync();
        }
        public async Task Add(Reservation reservation)
        {
            await _dbContext.AddAsync(reservation);
            await  _dbContext.SaveChangesAsync();
        }   

        public async Task<Reservation> GetById(int reservationId)
        {
            return await _dbContext.Reservations.FirstOrDefaultAsync(r => r.Id == reservationId);
        }
        public async Task SaveChanges()
        {
            await _dbContext.SaveChangesAsync();
        }
        public async Task<List<Reservation>> GetReservations()
        {
            return await _dbContext.Reservations.Include(x=>x.Desk).ToListAsync();
        }
    }


}
