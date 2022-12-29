using AutoMapper;
using DeskBookingSystem.Entities;
using DeskBookingSystem.Exceptions;
using DeskBookingSystem.Models;

namespace DeskBookingSystem.Services
{
    public interface IReservationsService
    {
        public int BookDesk(NewReservationDto newReservationDto);
        public bool ChangeDesk(int reservationId, int newDeskId);
    }
    public class ReservationsService : IReservationsService
    {
        private readonly BookingSystemDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IDateValidationService _dateValidationService;

        public ReservationsService(BookingSystemDbContext dbContext, IDateValidationService dateValidationService, IMapper mapper)
        {
            _dbContext = dbContext;
            _dateValidationService = dateValidationService;
            _mapper = mapper;
        }


        //Book desk with given ID for chosen time throws exception if there is no desk with given id or desk is not available
        public int BookDesk(NewReservationDto newReservationDto)
        {
            //use date validator to check if reservation is not too long, and does not start in past days
            var dateIsValid = _dateValidationService
                .DateIsValid(newReservationDto.ReservationStart, newReservationDto.ReservationEnd);
            //Check if there is desk with given id
            var desk = _dbContext.Desks
                .FirstOrDefault(d => d.Id == newReservationDto.DeskId);     
            if (desk == null) throw new DeskNotFoundException("Desk not found");
            //Check if there is no other reservation or  if desk is available with date validator
            var deskAvailableAtGivenTime = _dateValidationService
               .DeskIsAvailableAtGivenTime(desk.Id, newReservationDto.ReservationStart, newReservationDto.ReservationEnd);
            if (!desk.Available || !deskAvailableAtGivenTime)
            {
                throw new DeskNotAvaibleException("Desk is not available at given time");
            }
            var reservation = _mapper.Map<Reservation>(newReservationDto);
            _dbContext.Add(reservation);
            _dbContext.SaveChanges();
            return reservation.Id;

        }
        //Change desk to another return false if its too late or desk is not available, otherwise true
        public bool ChangeDesk(int reservationId, int newDeskId)
        {
            var reservation = _dbContext.Reservations
                .FirstOrDefault(r => r.Id == reservationId);
            if (reservation == null) return false;
            var hoursTillReservation = (reservation.ReservationStart - DateTime.Now).TotalHours;
            if (hoursTillReservation < 24) return false;
            var deskIsAvailable = _dateValidationService
                .DeskIsAvailableAtGivenTime(newDeskId,reservation.ReservationStart,reservation.ReservationEnd);
            if (!deskIsAvailable) return false;
            reservation.DeskId= newDeskId;
            _dbContext.SaveChanges();
            return true;
        }
    }
}
