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

        public int BookDesk(NewReservationDto newReservationDto)
        {
            var dateIsValid = _dateValidationService
                .DateIsValid(newReservationDto.ReservationStart, newReservationDto.ReservationEnd);
            var desk = _dbContext.Desks
                .FirstOrDefault(d => d.Id == newReservationDto.DeskId);     
            if (desk == null) throw new DeskNotFoundException("Desk not found");
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
