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
        //5[Authorize(Roles ="Administrator")]
        public async Task<ActionResult> AddDesks([FromBody] NewDeskDto newDeskDto)
        {
            
            var id = await _desksService.AddDesk(newDeskDto);
            if (id > -1) return Created($"/api/DeskController/{id}",null);
            return BadRequest();
        }

        [HttpPut("{id}/{availability}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> ManageAvailbility([FromRoute] int id, [FromRoute] bool availability)
        {
            
            var isUpdated = await _desksService.ManageAvailability(id, availability);
            if (!isUpdated) return NotFound();
            return Ok();
            
        }
        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> Delete(int id)
        {
            var isDeleted = await _desksService.RemoveDesk(id);
            if (!isDeleted)
            {
                return BadRequest();
            }
            return NoContent();

        }
        [HttpGet("/GetDesksEmployee/{location}/{sinceWhen}/{tillWhen}")]
        [Authorize(Roles = "Employee")]
        public async Task<List<DeskDto>> GetDesksByLocation([FromRoute] string location, DateTime sinceWhen, DateTime tillWhen)
        {
            var desks = await _desksService.GetDesksByLocation(location, sinceWhen, tillWhen);
            return desks;
        }
        [HttpGet("/GetDeskAdmin/{location}/{sinceWhen}/{tillWhen}")]
        [Authorize(Roles = "Administrator")]
        public async Task<List<DeskDtoForAdmin>> GetDesksByLocationForAdmin([FromRoute] string location, DateTime sinceWhen, DateTime tillWhen)
        {
            var desks = await _desksService.GetDesksByLocationForAdmin(location, sinceWhen, tillWhen);
            return desks;
           
        }

    }
}
