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
    [Route("api/format")]
    [ApiController]
    public class FormatController : ControllerBase
    {
        private readonly MTGContext _context;

        private readonly IConfiguration _configuration;


        public FormatController(MTGContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [EnableCors("MainPolicy")]
        [HttpPost("add")]
        {
            public IActionResult AddFormat([FromBody] Format format)
        {
            try
            {
                var existingFormat = _context.Formats.Where(f => f.Name == format.Name).FirstOrDefault();
                if (existingFormat != null)
                {
                    if (format.Name != null && format.minCardNumber != null && format.maxCardNumber != null)
                    {
                        _context.Add(format);
                        _context.SaveChanges();
                        return Ok($"Format {format.Name} succesfully saved in database!");
                    }
                    else
                    {
                        return StatusCode(403, "Some must have parameters are missing.");
                    }
                }
                return StatusCode(409, "Format name already in the database");
            }
            catch (System.Exception e)
            {
                return StatusCode(400, e);
            }
        }
    }

}
}