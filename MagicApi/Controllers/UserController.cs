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

namespace MagicApi.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly MTGContext _context;

        public UserController(MTGContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [EnableCors("MainPolicy")]
        [HttpPost("register")]
        public IActionResult Register([FromBody] User model)
        {
            System.Console.WriteLine(model.Username);
            System.Console.WriteLine(model.Email);
            System.Console.WriteLine(model.Password);

            try
            {
                // create user
                _context.Add(model);
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(model.Username);
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}