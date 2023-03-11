using AutoMapper;
using DeskBookingSystem.Entities;
using DeskBookingSystem.Exceptions;
using DeskBookingSystem.Models;
using DeskBookingSystem.Repositories;
using DeskBookingSystem.Services;
using FluentAssertions;
using Moq;

namespace DeskBookingSystem.Tests.ServiceTests
{
    public class DeskServiceTests
    {
        private readonly DesksService _service;
        private readonly Mock<IMapper> _mapper = new();
        private readonly Mock<IDateValidationService> _dateValidatior = new();
        private readonly Mock<IDeskRepository> _deskRepository = new();
        private readonly Mock<ILocationRepository> _locationRepository = new();
        private readonly Mock<IReservationRepository> _reservationRepository = new();

        public DeskServiceTests()
        {
            _service = new(
                _mapper.Object,
                _dateValidatior.Object,
                _deskRepository.Object,
                _locationRepository.Object,
                _reservationRepository.Object);
        }


        //}
        [Fact]
        public async Task AddDesk_ForCorrectArguments_AddDesk()
        {
            //Arrange
            _locationRepository.Setup(x=>x.GetByName("Katowice")).ReturnsAsync(new Location() { Name="Katowice",Id=1});
            var newDeskDto = new NewDeskDto() { LocationName = "Katowice" };
            var newDesk = new Desk() { LocationId = 1 };
            _mapper.Setup(m=>m.Map<Desk>(newDeskDto)).Returns(newDesk); 

            //Act
            await _service.AddDesk(newDeskDto);

            //Assert
            _deskRepository.Verify(x => x.AddDesk(newDesk));

        }
        [Fact]
        public async Task AddDesk_ForNoLocation_ThrowsException()
        {
            //Arrange
            var newDeskDto = new NewDeskDto() { LocationName = "Wrocław" };          

            //Act
            var result = async () => await _service.AddDesk(newDeskDto);

            //Assert
           result.Should().ThrowAsync<LocationNotFoundException>();
        }
        
        [Fact]
        public async Task GetDesksByLocationForAdmin_ForCorrectArguments_ReturnDtos()
        {
            //Arrange
            var location = new Location() { Name = "Katowice", Id = 1};
            var desk = new Desk() { Id = 1, Location = location, Available=true,LocationId=1 };
            var user = new User() { Id= 1 , Name="Przemek"};
            var reservation = new Reservation() { Desk= desk, ReservationStart = DateTime.Now.AddMinutes(10),ReservationEnd= DateTime.Now.AddDays(2),DeskId=1, Id=1, UserId=1 ,User=user};
            _locationRepository.Setup(x => x.GetByName("Katowice")).ReturnsAsync(location);
            _deskRepository.Setup(x=>x.GetDesks()).ReturnsAsync(new List<Desk> { desk });
            _reservationRepository.Setup(x => x.GetReservations()).ReturnsAsync(new List<Reservation> { reservation });

            //Act
            var result = await _service.GetDesksByLocationForAdmin("Katowice", DateTime.Now, DateTime.Now.AddDays(2));

            //Assert
            result.Should().BeOfType<List<DeskDtoForAdmin>>();
            result.Count.Should().Be(1);

        }
        

        [Fact]
        public async Task GetDesksByLocationForAdmin_ForNoLocation_ThrowsException()
        {
            //Act
            var result = async() => await _service.GetDesksByLocationForAdmin("Tokyo",DateTime.Now,DateTime.Now.AddDays(2));

            //Assert
            result.Should().ThrowAsync<LocationNotFoundException>();
            
        }

        //public async Task<List<DeskDto>> GetDesksByLocation(string locationName, DateTime sinceWhen, DateTime tillWhen)
        //{
        //    var location = _locationRepository.GetByName(locationName);
        //    if (location == null) throw new LocationNotFoundException("There is no location with given name");
        //    //Left join to desks with given location desks where there is some reservation at given time
        //    //desks where is reservation or availability is set to false, will be listed as unavailable
        //    var desks = from desk in (await _deskRepository.GetDesks())
        //                .Where(d => d.Location.Name == locationName)
        //                join reservations in (await _reservationRepository.GetReservations())
        //                .Where(r => r.ReservationStart <= sinceWhen
        //                && r.ReservationEnd >= tillWhen)
        //                on desk equals reservations.Desk into gj
        //                from subreservation in gj.DefaultIfEmpty()
        //                select new DeskDto
        //                {
        //                    LocationName = desk.Location.Name,
        //                    Id = desk.Id,
        //                    Available = subreservation != null || desk.Available == false ? false : true
        //                };


        //    return desks.ToList();

        //}
        [Fact]
        public async Task GetDesksByLocation_ForCorrectArguments_ReturnDtos()
        {
            //Arrange
            var location = new Location() { Name = "Katowice", Id = 1 };
            var desk = new Desk() { Id = 1, Location = location, Available = true, LocationId = 1 };
            var user = new User() { Id = 1, Name = "Przemek" };
            var reservation = new Reservation() { Desk = desk, ReservationStart = DateTime.Now.AddMinutes(10), ReservationEnd = DateTime.Now.AddDays(2), DeskId = 1, Id = 1, UserId = 1, User = user };
            _locationRepository.Setup(x => x.GetByName("Katowice")).ReturnsAsync(location);
            _deskRepository.Setup(x => x.GetDesks()).ReturnsAsync(new List<Desk> { desk });
            _reservationRepository.Setup(x => x.GetReservations()).ReturnsAsync(new List<Reservation> { reservation });

            //Act
            var result = await _service.GetDesksByLocation("Katowice", DateTime.Now, DateTime.Now.AddDays(2));

            //Assert
            result.Should().BeOfType<List<DeskDto>>();
            result.Count.Should().Be(1);

        }


        [Fact]
        public async Task GetDesksByLocation_ForNoLocation_ThrowsException()
        {
            //Act
            var result = async () => await _service.GetDesksByLocation("Tokyo", DateTime.Now, DateTime.Now.AddDays(2));

            //Assert
            result.Should().ThrowAsync<LocationNotFoundException>();

        }
    }
}
    