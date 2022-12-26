using DeskBookingSystem.Entities;
using DeskBookingSystem.Exceptions;
using DeskBookingSystem.Models;

namespace DeskBookingSystem.Services
{
    public interface IAvailabilityService
    {
        public bool DateIsAvailable(Reservation reservation);
    }
    public class AvailabilityService : IAvailabilityService
{
        private readonly BookingSystemDbContext _dbContext;

        public AvailabilityService(BookingSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool DateIsAvailable(Reservation reservation)
        {
            
            var conflictingReservations = _dbContext.Reservations
                //Reservations which ends during given reservation
                .Where(r=> (r.ReservationEnd >= reservation.ReservationStart && r.ReservationEnd <= reservation.ReservationEnd
                //Reservations which starts during given reservation
                || r.ReservationStart<=reservation.ReservationEnd)
                && r.DeskId == reservation.DeskId)
                .ToList();
            if (conflictingReservations != null) return false;
            return true;
            
        }
        public bool DateIsValid(Reservation reservation)
        {
            var dateIsValid = (reservation.ReservationStart > DateTime.Now
                                && reservation.ReservationEnd > reservation.ReservationStart);
            if (!dateIsValid)
            {
                return false;
            }
            return true;
        }
        
    }
}
