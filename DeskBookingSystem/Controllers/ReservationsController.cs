using DeskBookingSystem.Models;
using DeskBookingSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeskBookingSystem.Controllers
{
    [Route("api/ReservationController")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationsService _reservationsService;

        public ReservationsController(IReservationsService reservationsService)
        {
            _reservationsService = reservationsService;
        }
        [HttpPost]
        [Authorize(Roles = "Employee")]
        public ActionResult BookDesk([FromBody] NewReservationDto newReservationDto)
        {
            var id = _reservationsService.BookDesk(newReservationDto);
            if (id > -1) return Created($"/api/ReservationController/{id}", null);
            return BadRequest();
        }

        [HttpPut("{reservationId}/{newDeskId}")]
        [Authorize(Roles = "Administrator")]
        public ActionResult ChangeDesk(int reservationId, int newDeskId)
        {
            var isChanged = _reservationsService.ChangeDesk(reservationId, newDeskId);
            if (!isChanged) return BadRequest();
            return Ok();

        }
    }
}
