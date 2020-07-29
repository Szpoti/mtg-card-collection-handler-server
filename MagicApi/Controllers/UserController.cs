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

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, _configuration.GetSection("BCrypt").GetSection("salt").Value);
        }
    }
}