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

        public UserService(BookingSystemDbContext dbContext, IMapper mapper, IPasswordHasher<User> passwordHasher, AuthenticationSettings authenticationSettings)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _authenticationSettings = authenticationSettings;
        }

        //Create user, hash given password and add to database
        public int Create(NewUserDto newUserDto)
        {
            var newUser = _mapper.Map<User>(newUserDto);
            var hashedPasswd = _passwordHasher.HashPassword(newUser, newUserDto.Password);
            newUser.PasswordHash = hashedPasswd;
            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();



            return newUser.Id;

        }


        //Login service check if there is user with given password and email and returns JWT token
        public string GenerateJwt(LoginDto loginDto)
        {
            //Find user with same email as given, join roles
            var user = _dbContext.Users
                .Include(x => x.Role)
                .FirstOrDefault(x => x.Email == loginDto.Email);
            if (user == null)
            {
                throw new BadRequestException("Invalid username or password");
            }
            //Check if password after hash match hashed password of user in database
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("Invalid username or password");

            }
            //Make list of claims used for authentication 
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.Name}"),
                new Claim(ClaimTypes.Role,user.Role.Name),



            };
            //Private key based on appsettings.json
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
            //signing credentials hashed with HmacSha256 algorithm
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //expiration date, set in appsetting.json
            var expires = DateTime.UtcNow.AddDays(_authenticationSettings.JwtExpireDays);

            //Generating new JWT token
            var token = new JwtSecurityToken(_authenticationSettings.JwtIssuer,
                _authenticationSettings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: cred);
            //Returning Token as string
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
    }
}