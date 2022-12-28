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
        public ActionResult CreateUser([FromBody] NewUserDto newUser)
        {
            
            var id = _userService.Create(newUser);
            return Created($"/api/UserController/{id}", null);
        }
        
        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginDto loginDto)
        {
            string token = _userService.GenerateJwt(loginDto);
            return Ok(token);
        }

    }
}