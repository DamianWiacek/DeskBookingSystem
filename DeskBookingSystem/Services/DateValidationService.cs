﻿using DeskBookingSystem.Entities;
using DeskBookingSystem.Exceptions;
using DeskBookingSystem.Models;

namespace DeskBookingSystem.Services
{
    public interface IDateValidationService
    {
        public bool DeskIsAvailableAtGivenTime(int deskId, DateTime reservationStart, DateTime reservationEnd);
        bool DateIsValid(DateTime reservationStart, DateTime reservationEnd);
    }
    public class DateValidationService : IDateValidationService
    {
        private readonly BookingSystemDbContext _dbContext;

        public DateValidationService(BookingSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        //Method check if there are no other reservations for given DeskId in given time
        public bool DeskIsAvailableAtGivenTime(int deskId, DateTime reservationStart, DateTime reservationEnd)
        {
            var desk = _dbContext.Desks
                .FirstOrDefault(d => d.Id == deskId);
            if (desk == null) return false;
            else if(desk.Available == false) return false;
            var conflictingReservations = _dbContext.Reservations
                //Reservations which ends during given reservation
                .FirstOrDefault(r => (r.ReservationEnd >= reservationStart && r.ReservationEnd <= reservationEnd
                //Reservations which starts during given reservation
                || (r.ReservationStart >= reservationStart && r.ReservationStart <= reservationEnd))
                && r.DeskId == deskId);
            
            if (conflictingReservations != null) return false;
            return true;
            
        }

        //For given reservation start and end checks if reservation duration is not longer than 7 days,
        //if reservation start is greater than its end and if its start is greater than current time so 
        //employee cannot add reservation with past day
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
