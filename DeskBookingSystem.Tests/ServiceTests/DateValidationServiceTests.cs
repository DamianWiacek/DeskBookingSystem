using DeskBookingSystem.Entities;
using DeskBookingSystem.Repositories;
using DeskBookingSystem.Services;
using FluentAssertions;
using Moq;

namespace DeskBookingSystem.Tests.ServiceTests
{
    public class DateValidationServiceTests
    {
        private readonly DateValidationService _service;
        private readonly Mock<IReservationRepository> _reservationRepository = new();
        private readonly Mock<IDeskRepository> _deskRepository = new();
        public DateValidationServiceTests()
        {
            _service = new(_deskRepository.Object, _reservationRepository.Object);
        }


        [Theory]
        [InlineData(2, "02/11/2023", "02/13/2023")]
        [InlineData(1, "01/22/2023", "01/24/2023")]
        [InlineData(1, "01/2/2023", "01/16/2023")]
        public async Task DeskIsAvailableAtGivenTime_ForNotCorrectArguments_ReturnFalse(int deskId, string reservationStart, string reservationEnd)
        {
            //Arrange
            _deskRepository.Setup(x => x.GetDeskById(1)).ReturnsAsync((Desk)null);
            var reservationList = new List<Reservation>
            {
                new Reservation() { DeskId = deskId, ReservationStart = DateTime.Parse(reservationStart), ReservationEnd = DateTime.Parse(reservationEnd) }
            };
            _reservationRepository.Setup(x => x.GetAll()).ReturnsAsync(reservationList);
            var desk = await _deskRepository.Object.GetDeskById(1);

            //Act
            var result = await _service.DeskIsAvailableAtGivenTime(deskId, DateTime.Parse(reservationStart), DateTime.Parse(reservationEnd));

            //Assert
            result.Should().Be(false);
        }

        [Theory]
        [InlineData(1, "01/22/2023", "01/24/2023")]
        [InlineData(1, "01/2/2023", "01/6/2023")]
        public async Task DeskIsAvailableAtGivenTime_ForCorrectArguments_ReturnTrue(int deskId, string reservationStart, string reservationEnd)
        {
            //Arrange
            _deskRepository.Setup(x => x.GetDeskById(1)).ReturnsAsync(new Desk() { Id=1,Available=true});
            var reservationList = new List<Reservation>
            {
                new Reservation() { DeskId = deskId, ReservationStart = DateTime.Parse("01/8/2023"), ReservationEnd = DateTime.Parse("01/14/2023") }
            };
            _reservationRepository.Setup(x => x.GetAll()).ReturnsAsync(reservationList);
            var desk = await _deskRepository.Object.GetDeskById(1);

            //Act
            var result = await _service.DeskIsAvailableAtGivenTime(deskId, DateTime.Parse(reservationStart), DateTime.Parse(reservationEnd));

            //Assert
            result.Should().Be(true);
        }
    }
}
