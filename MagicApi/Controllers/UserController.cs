using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.Extensions.Options;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using MagicApi.Models;
using MagicApi.Services;
using MagicApi.Data;
using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace MagicApi.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly MTGContext _context;

        private readonly IConfiguration _configuration;

        private readonly JwtService _authService;

        public UserController(MTGContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _authService = new JwtService(_configuration);
        }

        [AllowAnonymous]
        [EnableCors("MainPolicy")]
        [HttpPost("register")]
        public User Register([FromBody] User model)
        {
            try
            {
                // create user
                model.Password = HashPassword(model.Password);
                model.JWT = _authService.GenerateSecurityToken(model);
                _context.Add(model);
                _context.SaveChanges();
                model.Password = null;
                return model;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [AllowAnonymous]
        [EnableCors("MainPolicy")]
        [HttpPost("login")]
        public User Login([FromBody] User model)
        {
            try
            {
                var user = _context.Users.Where(u => u.Email == model.Email).FirstOrDefault();
                if (CheckPassword(model.Password, user.Password))
                {
                    user.JWT = _authService.GenerateSecurityToken(user);
                    _context.SaveChanges();
                    user.Password = string.Empty;
                    return user;
                }
                return null;
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                System.Console.WriteLine(ex.Message);
                return null;
            }
        }

        [Authorize]
        [EnableCors("MainPolicy")]
        [HttpGet("login")]
        public User GetUser(string jwt)
        {
            return _context.Users
                .Where(user => user.JWT == jwt)
                .Select(u => new User() { Username = u.Username })
                .FirstOrDefault();
        }

        [AllowAnonymous]
        [EnableCors("MainPolicy")]
        [HttpPost("logout")]
        public IActionResult LogOut(string jwt)
        {
            User user =_context.Users
                .Where(user => user.JWT == jwt)
                .FirstOrDefault();
            if (user == null)
            {
                return Ok();
            }

            user.JWT = "";
            _context.SaveChanges();
            return Ok();
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, _configuration.GetSection("BCrypt").GetSection("salt").Value);
        }

        private bool CheckPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}