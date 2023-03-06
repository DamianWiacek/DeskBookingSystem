using DeskBookingSystem.Models;
using DeskBookingSystem.Services;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Administrator")]
    public class LocationsController : ControllerBase
    {
        private IlocationService _locationService;

        public LocationsController(IlocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpPost]
        public async Task<ActionResult> AddNewLocation([FromBody] NewLocationDto newLocationDto)
        {
            var id = await _locationService.AddLocation(newLocationDto);
            return Created($"/api/LocationController/{id}", null);
        }
        [HttpDelete("{name}")]
        public async Task<ActionResult> Delete([FromRoute]string name)
        {
            var isDeleted = await _locationService.RemoveLocation(name);
            if (isDeleted) return NoContent();
            return NotFound();
           
        }

    }
}
