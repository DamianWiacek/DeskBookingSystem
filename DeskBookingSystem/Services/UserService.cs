using AutoMapper;
using DeskBookingSystem.Entities;
using DeskBookingSystem.Exceptions;
using DeskBookingSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DeskBookingSystem.Services
{
    public interface IUserService
    {
        public int Create(NewUserDto newUserDto);
        public string GenerateJwt(LoginDto loginDto);

    }
    public class UserService : IUserService
    {
        private readonly BookingSystemDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly AuthenticationSettings _authenticationSettings;

        public UserService(BookingSystemDbContext dbContext, IMapper mapper, ILogger<UserService> logger, IPasswordHasher<User> passwordHasher, AuthenticationSettings authenticationSettings)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _authenticationSettings = authenticationSettings;
        }

        
        public int Create(NewUserDto newUserDto)
        {
            var newUser = _mapper.Map<User>(newUserDto);
            var hashedPasswd = _passwordHasher.HashPassword(newUser, newUserDto.Password);
            newUser.PasswordHash = hashedPasswd;
            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();



            return newUser.Id;

        }



        public string GenerateJwt(LoginDto loginDto)
        {
            var user = _dbContext.Users.Include(x => x.Role).FirstOrDefault(x => x.Email == loginDto.Email);
            if (user == null)
            {
                throw new BadRequestException("Invalid username or password");
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("Invalid username or password");

            }
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.Name}"),
                new Claim(ClaimTypes.Role,user.Role.Name),



            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddDays(_authenticationSettings.JwtExpireDays);

            var token = new JwtSecurityToken(_authenticationSettings.JwtIssuer,
                _authenticationSettings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: cred);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
    }
}