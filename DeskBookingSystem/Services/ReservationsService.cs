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
        private readonly IAvailabilityService _availabilityService;

        public ReservationsService(BookingSystemDbContext dbContext, IAvailabilityService availabilityService, IMapper mapper)
        {
            _dbContext = dbContext;
            _availabilityService = availabilityService;
            _mapper = mapper;
        }

        public int BookDesk(NewReservationDto newReservationDto)
        {
            var dateIsValid = _availabilityService
                .DateIsValid(newReservationDto.ReservationStart, newReservationDto.ReservationEnd);
            var desk = _dbContext.Desks
                .FirstOrDefault(d => d.Id == newReservationDto.DeskId);     
            if (desk == null) throw new DeskNotFoundException("Desk not found");
            var deskAvailableAtGivenTime = _availabilityService
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
            var deskIsAvailable = _availabilityService
                .DeskIsAvailableAtGivenTime(newDeskId,reservation.ReservationStart,reservation.ReservationEnd);
            if (!deskIsAvailable) return false;
            reservation.DeskId= newDeskId;
            _dbContext.SaveChanges();
            return true;
        }
    }
}
