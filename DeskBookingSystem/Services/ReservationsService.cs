using AutoMapper;
using DeskBookingSystem.Entities;
using DeskBookingSystem.Exceptions;
using DeskBookingSystem.Models;
using DeskBookingSystem.Repositories;

namespace DeskBookingSystem.Services
{
    public interface IReservationsService
    {
        public Task<int> BookDesk(NewReservationDto newReservationDto);
        public Task<bool> ChangeDesk(int reservationId, int newDeskId);
    }
    public class ReservationsService : IReservationsService
    {
        private readonly IMapper _mapper;
        private readonly IDeskRepository _deskRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IDateValidationService _dateValidationService;

        public ReservationsService(BookingSystemDbContext dbContext, IDateValidationService dateValidationService, IMapper mapper, IDeskRepository deskRepository, IReservationRepository reservationRepository)
        {
            _dateValidationService = dateValidationService;
            _mapper = mapper;
            _deskRepository = deskRepository;
            _reservationRepository = reservationRepository;
        }


        //Book desk with given ID for chosen time throws exception if there is no desk with given id or desk is not available
        public async Task<int> BookDesk(NewReservationDto newReservationDto)
        {
            //use date validator to check if reservation is not too long, and does not start in past days
            var dateIsValid = await _dateValidationService
                .DateIsValid(newReservationDto.ReservationStart, newReservationDto.ReservationEnd);
            //Check if there is desk with given id
            var desk = await _deskRepository.GetDeskById(newReservationDto.DeskId);
           
            if (desk == null) throw new DeskNotFoundException("Desk not found");
            //Check if there is no other reservation or  if desk is available with date validator
            var deskAvailableAtGivenTime = await _dateValidationService
               .DeskIsAvailableAtGivenTime(desk.Id, newReservationDto.ReservationStart, newReservationDto.ReservationEnd);
            if (!desk.Available || !deskAvailableAtGivenTime)
            {
                throw new DeskNotAvaibleException("Desk is not available at given time");
            }
            var reservation = _mapper.Map<Reservation>(newReservationDto);
            await _reservationRepository.Add(reservation);
            return reservation.Id;

        }
        //Change desk to another return false if its too late or desk is not available, otherwise true
        public async Task<bool> ChangeDesk(int reservationId, int newDeskId)
        {
            var reservation = await _reservationRepository.GetById(reservationId);
            if (reservation == null) return false;
            var hoursTillReservation = (reservation.ReservationStart - DateTime.Now).TotalHours;
            if (hoursTillReservation < 24) return false;
            var deskIsAvailable = await _dateValidationService
                .DeskIsAvailableAtGivenTime(newDeskId,reservation.ReservationStart,reservation.ReservationEnd);
            if (!deskIsAvailable) return false;
            reservation.DeskId= newDeskId;
            await _reservationRepository.SaveChanges();
            return true;
        }
    }
}
