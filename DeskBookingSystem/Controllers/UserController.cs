using DeskBookingSystem.Models;
using DeskBookingSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeskBookingSystem.Controllers
{
    [ApiController]
    [Route("api/UserController")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

       

        [HttpPost]
        public async Task<ActionResult> CreateUser([FromBody] NewUserDto newUser)
        {
            
            var id = await _userService.Create(newUser);
            return Created($"/api/UserController/{id}", null);
        }
        
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
        {
            var logedUser = await _userService.GetLogedUser(loginDto);
            return Ok(logedUser);
            
        }

    }
}