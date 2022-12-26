using DeskBookingSystem.Models;
using DeskBookingSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeskBookingSystem.Controllers
{
    [Route("api/LocationController")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private IlocationService _locationService;

        public LocationsController(IlocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpPost]
        public ActionResult AddNewLocation([FromBody] NewLocationDto newLocationDto)
        {
            var id = _locationService.AddLocation(newLocationDto);
            return Created($"/api/LocationController/{id}", null);
        }
        [HttpDelete("{name}")]
        public ActionResult Delete([FromRoute]string name)
        {
            var isDeleted = _locationService.RemoveLocation(name);
            if (isDeleted == "Deleted")
            {
                return NoContent();
            }
            else if (isDeleted == "BadRequest")
            {
                return BadRequest();
            }
            else
            {
                return NotFound();
            }

        }

    }
}
