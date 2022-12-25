using DeskBookingSystem.Models;
using DeskBookingSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeskBookingSystem.Controllers
{
    [Route("api/ReservationController")]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationsService _reservationsService;

        public ReservationsController(IReservationsService reservationsService)
        {
            _reservationsService = reservationsService;
        }
        [HttpPost]
        public ActionResult BookDesk([FromBody] NewReservationDto newReservationDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var id = _reservationsService.BookDesk(newReservationDto);
            if (id > -1) return Created($"/api/ReservationController/{id}", null);
            return BadRequest();
        }

        [HttpPut("{reservationId}/{newDeskId}")]
        public ActionResult ChangeDesk(int reservationId, int newDeskId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var isChanged = _reservationsService.ChangeDesk(reservationId, newDeskId);
            if (!isChanged) return BadRequest();
            return Ok();

        }
    }
}
