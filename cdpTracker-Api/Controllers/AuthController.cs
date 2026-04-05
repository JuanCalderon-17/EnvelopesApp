using cdpTracker_Api.Data;
using cdpTracker_Api.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace cdpTracker_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config; // inject IConfiguration and read Key of appsettings.json
        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            //Find the worker by name using LINQ
            var worker =  await _context.Workers
                .FirstOrDefaultAsync(w => w.Name == request.Name && w.PasswordHash == request.Password);

            //Check if worker exists and password matches
            if (worker == null) return Unauthorized("Invalid username or password.");

            //create the claims 
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, worker.Id.ToString()),
                new Claim(ClaimTypes.Name, worker.Name),
                new Claim(ClaimTypes.Role, worker.Role.ToString()),
                new Claim("kiosko", worker.Kiosko.ToString())
            };


            //generate secret signature key
            //create the token




            // If authentication is successful, return token and worker info
            return Ok(new
            {
                worker.Id,
                worker.Name,
                worker.Role,
                worker.Kiosko,

            });
        }
    }
}
