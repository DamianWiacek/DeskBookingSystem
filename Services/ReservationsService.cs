using AutoMapper;
using DeskBookingSystem.Entities;
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
        private readonly IAvailabilityService _availabilitySerrvice;

        public ReservationsService(BookingSystemDbContext dbContext, IAvailabilityService availabilitySerrvice, IMapper mapper)
        {
            _dbContext = dbContext;
            _availabilitySerrvice = availabilitySerrvice;
            _mapper = mapper;
        }

        public int BookDesk(NewReservationDto newReservationDto)
        {
            var reservationDuration = newReservationDto.ReservationEnd - newReservationDto.ReservationStart;
            var currentTime = newReservationDto.ReservationStart > DateTime.Now;
            var desk = _dbContext.Desks
                .FirstOrDefault(d => d.Id == newReservationDto.DeskId);
            var deskReservation = _dbContext.Reservations
                .FirstOrDefault(r => r.ReservationEnd >= newReservationDto.ReservationStart
                && r.ReservationStart <= newReservationDto.ReservationStart
                && r.DeskId == newReservationDto.DeskId);
                
            if (reservationDuration.TotalDays > 7 || desk == null) return -1;
            else if(desk.Available == false || deskReservation != null ) return -1;
            var reservation = _mapper.Map<Reservation>(newReservationDto);
            _dbContext.Add(reservation);
            _dbContext.SaveChanges();
            return reservation.Id;

        }

        public bool ChangeDesk(int reservationId, int newDeskId)
        {
            var newDesk = _dbContext.Desks
                .FirstOrDefault(d => d.Id == newDeskId);
            var reservation = _dbContext.Reservations
                .FirstOrDefault(r => r.Id == reservationId);
            if (newDesk == null || reservation == null) return false;
            var deskReservation = _dbContext.Reservations
                .FirstOrDefault(r => r.ReservationEnd >= reservation.ReservationStart
                && r.ReservationStart <= reservation.ReservationStart
                && r.DeskId == newDeskId);
            if (newDesk.Available == false || deskReservation != null) return false;
            reservation.DeskId= newDeskId;
            _dbContext.SaveChanges();
            return true;
        }
    }
}
