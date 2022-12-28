using DeskBookingSystem.Entities;
using DeskBookingSystem.Services;
using DeskBookingSystem.Entities;

using FakeItEasy;
using Moq;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using DeskBookingSystem.Exceptions;
using DeskBookingSystem.Models;

namespace DeskBookingSystem.Tests
{
    public class DateValidationService
    {
        private BookingSystemDbContext _dbContext;

        public DateValidationService()
        {
            var builder = new DbContextOptionsBuilder<BookingSystemDbContext>();
            builder.UseInMemoryDatabase("TestDb");

            _dbContext = new BookingSystemDbContext(builder.Options);
            Seed();
        }

        public void Seed()
        {
            var testLocations = new List<Location>()
            {
                new Location()
                {
                   
                    Name = "Katowice" 
                },
                 new Location()
                {
                     
                    Name = "Kraków"
                },
            };
            var testDesks = new List<Desk>()
            {
                new Desk()
                {
                    
                    LocationId = 1,
                    Available = true,

                }
            };
            var testReservations = new List<Reservation>()
            {
                new Reservation()
                {

                    DeskId = 1,
                    UserId = 1,
                    ReservationStart = DateTime.Parse("01/22/2023"),
                    ReservationEnd = DateTime.Parse("01/24/2023")
                }
            };

            _dbContext.Locations.AddRange(testLocations);
            _dbContext.Desks.AddRange(testDesks);
            _dbContext.SaveChanges();
        }

        [Fact]
        public void DateIsVaild_ForCorrectDate_ReturnTrue()
        {
            //arrange
            var service = new Services.DateValidationService(_dbContext);
            //act
            var result =service.DateIsValid(DateTime.Now.AddDays(1),DateTime.Now.AddDays(2));

            //assert

            Assert.True(result);
        }
        [Fact]
        public void DateIsVaild_ForIncorrectDate_ReturnFalse()
        {
            //arrange
            var service = new Services.DateValidationService(_dbContext);
            //act
            var result = service.DateIsValid(DateTime.Now.AddDays(6), DateTime.Now.AddDays(3));

            //assert

            Assert.False(result);
        }


        //Date form : MM/DD/YYYY
        [Theory]
        [InlineData(1,"01/22/2023","01/24/2023")]
        [InlineData(1,"01/2/2023","01/6/2023")]
        public void DeskIsAvailableAtGivenTime_ForValidData_ReturnTrue(int deskId, string reservationStart, string reservationEnd)
        {
            //arrange
            var service = new Services.DateValidationService(_dbContext);

            //act
            var result = service.DeskIsAvailableAtGivenTime(deskId,DateTime.Parse(reservationStart), DateTime.Parse(reservationEnd));

            //assert

            Assert.True(result);    
        }

        [Theory]
        [InlineData(2, "02/11/2023", "02/13/2023")]
        [InlineData(1, "01/22/2023", "01/24/2023")]
        [InlineData(1, "01/2/2023", "01/16/2023")]
        public void DeskIsAvailableAtGivenTime_ForNotValidData_ReturnFalse(int deskId, string reservationStart, string reservationEnd)
        {
            //arrange
            var service = new Services.DateValidationService(_dbContext);

            //act
            var result = service.DeskIsAvailableAtGivenTime(deskId, DateTime.Parse(reservationStart), DateTime.Parse(reservationEnd));

            //assert

            Assert.True(result);
        }
    }
}