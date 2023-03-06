using DeskBookingSystem.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeskBookingSystem.Repositories
{
    public interface IReservationRepository
    {
        public IQueryable<Reservation> GetAll();
        public void Add(Reservation reservation);
        public Reservation GetById(int reservationId);
        public void SaveChanges();
        public IQueryable<Reservation> GetReservations();
    }
    public class ReservationRepository : IReservationRepository
    {
        private readonly BookingSystemDbContext _dbContext;

        public ReservationRepository(BookingSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IQueryable<Reservation> GetAll()
        {
            return _dbContext.Reservations;
        }
        public void Add(Reservation reservation)
        {
            _dbContext.Add(reservation);
            _dbContext.SaveChanges();
        }

        public Reservation GetById(int reservationId)
        {
            return _dbContext.Reservations.FirstOrDefault(r => r.Id == reservationId);
        }
        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
        public IQueryable<Reservation> GetReservations()
        {
            return _dbContext.Reservations;
        }
    }


}
