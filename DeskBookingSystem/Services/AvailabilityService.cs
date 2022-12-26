using DeskBookingSystem.Entities;
using DeskBookingSystem.Exceptions;
using DeskBookingSystem.Models;

namespace DeskBookingSystem.Services
{
    public interface IAvailabilityService
    {
        public bool DeskIsAvailableAtGivenTime(int deskId, DateTime reservationStart, DateTime reservationEnd);
        bool DateIsValid(DateTime reservationStart, DateTime reservationEnd);
    }
    public class AvailabilityService : IAvailabilityService
{
        private readonly BookingSystemDbContext _dbContext;

        public AvailabilityService(BookingSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        //Method check if there are no other reservations for given DeskId in given time
        public bool DeskIsAvailableAtGivenTime(int deskId, DateTime reservationStart, DateTime reservationEnd)
        {
            var conflictingReservations = _dbContext.Reservations
                //Reservations which ends during given reservation
                .FirstOrDefault(r => (r.ReservationEnd >= reservationStart && r.ReservationEnd <= reservationEnd
                //Reservations which starts during given reservation
                || (r.ReservationStart >= reservationStart && r.ReservationStart <= reservationEnd))
                && r.DeskId == deskId);
                
            if (conflictingReservations != null) return false;
            return true;
            
        }
        public bool DateIsValid(DateTime reservationStart, DateTime reservationEnd)
        {
            var dateIsValid = (reservationStart > DateTime.Now
                                && reservationEnd > reservationStart);
            var reservationDuration = (reservationEnd - reservationStart).TotalDays;
            if (!dateIsValid || reservationDuration>7)
            {
                return false;
            }
            return true;
        }
        
    }
}
