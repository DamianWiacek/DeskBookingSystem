using AutoMapper;
using DeskBookingSystem.Entities;
using DeskBookingSystem.Exceptions;
using DeskBookingSystem.Models;
using DeskBookingSystem.Repositories;
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
        public Task<int> Create(NewUserDto newUserDto);
        public Task<string> GenerateJwt(LoginDto loginDto);
        public Task<LogedUserDto> GetLogedUser(LoginDto loginDto);

    }
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly AuthenticationSettings _authenticationSettings;
        private readonly IUserRepository _userRepository;

        public UserService(BookingSystemDbContext dbContext, IMapper mapper, IPasswordHasher<User> passwordHasher, AuthenticationSettings authenticationSettings, IUserRepository userRepository)
        {
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _authenticationSettings = authenticationSettings;
            _userRepository = userRepository;
        }

        //Create user, hash given password and add to database
        public async Task<int> Create(NewUserDto newUserDto)
        {
            var newUser = _mapper.Map<User>(newUserDto);
            var hashedPasswd = _passwordHasher.HashPassword(newUser, newUserDto.Password);
            newUser.PasswordHash = hashedPasswd;
            await _userRepository.AddUser(newUser);

            return newUser.Id;

        }


        //Login service check if there is user with given password and email and returns JWT token
        public async Task<string> GenerateJwt(LoginDto loginDto)
        {
            //Find user with same email as given, join roles
            var user = await _userRepository.GetUserByEmail(loginDto.Email);
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

        public async Task<LogedUserDto> GetLogedUser(LoginDto loginDto)
        {
            //Find user with same email as given
            var user = await _userRepository.GetUserByEmail(loginDto.Email);
            if (user == null)
            {
                throw new BadRequestException("Invalid username or password");
            }
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("Invalid username or password");

            }

            return new LogedUserDto()
                                {
                                    Name = user.Name,
                                    Email = user.Email,
                                    RoleName = user.Role.Name,
                                    Token = await GenerateJwt(loginDto),
                                };
            
        }
    }

}   