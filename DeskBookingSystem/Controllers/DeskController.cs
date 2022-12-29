using DeskBookingSystem.Models;
using DeskBookingSystem.Services;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles ="Administrator")]
        public ActionResult AddDesks([FromBody] NewDeskDto newDeskDto)
        {
            
            var id = _desksService.AddDesk(newDeskDto);
            if (id > -1) return Created($"/api/DeskController/{id}",null);
            return BadRequest();
        }

        [HttpPut("{id}/{availability}")]
        [Authorize(Roles = "Administrator")]
        public ActionResult ManageAvailbility([FromRoute] int id, [FromRoute] bool availability)
        {
            
            var isUpdated = _desksService.ManageAvailability(id, availability);
            if (!isUpdated) return NotFound();
            return Ok();
            
        }
        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int id)
        {
            var isDeleted = _desksService.RemoveDesk(id);
            if (!isDeleted)
            {
                return BadRequest();
            }
            return NoContent();

        }
        [HttpGet("/GetDesksEmployee/{location}/{sinceWhen}/{tillWhen}")]
        [Authorize(Roles = "Employee")]
        public List<DeskDto> GetDesksByLocation([FromRoute] string location, DateTime sinceWhen, DateTime tillWhen)
        {
            var desks = _desksService.GetDesksByLocation(location, sinceWhen, tillWhen);
            return desks;
        }
        [HttpGet("/GetDeskAdmin/{location}/{sinceWhen}/{tillWhen}")]
        [Authorize(Roles = "Administrator")]
        public List<DeskDtoForAdmin> GetDesksByLocationForAdmin([FromRoute] string location, DateTime sinceWhen, DateTime tillWhen)
        {
            var desks = _desksService.GetDesksByLocationForAdmin(location, sinceWhen, tillWhen);
            return desks;
           
        }

    }
}
