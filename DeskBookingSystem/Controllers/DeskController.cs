using DeskBookingSystem.Models;
using DeskBookingSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeskBookingSystem.Controllers
{
    [Route("api/DeskController")]
    [ApiController]
    public class DeskController : ControllerBase
    {
        private readonly IDesksService _desksService;

        public DeskController(IDesksService desksService)
        {
            _desksService = desksService;
        }
        [HttpPost]
        public ActionResult AddDesks([FromBody] NewDeskDto newDeskDto)
        {
            
            var id = _desksService.AddDesk(newDeskDto);
            if (id > -1) return Created($"/api/DeskController/{id}",null);
            return BadRequest();
        }

        [HttpPut("{id}/{availability}")]
        public ActionResult ManageAvailbility([FromRoute] int id, [FromRoute] bool availability)
        {
            
            var isUpdated = _desksService.ManageAvailability(id, availability);
            if (!isUpdated) return NotFound();
            return Ok();
            
        }
        
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var isDeleted = _desksService.RemoveDesk(id);
            if (!isDeleted)
            {
                return BadRequest();
            }
            return NoContent();

        }
        [HttpGet("{location}")]
        public List<DeskDto> GetDesks([FromRoute] string location)
        {
            var desks = _desksService.GetDesks(location);
            return desks;
        }
        [HttpGet]
        public List<DeskDto> GetAllDesks()
        {
            var desks = _desksService.GetAllDesks();
            return desks;
        }

    }
}
