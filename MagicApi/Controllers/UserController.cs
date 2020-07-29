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

        public UserController(MTGContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [EnableCors("MainPolicy")]
        [HttpPost("register")]
        public IActionResult Register([FromBody] User model)
        {

            try
            {
                // create user
                model.Password = HashPassword(model.Password);
                _context.Add(model);
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
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