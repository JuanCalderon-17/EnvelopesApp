using cdpTracker_Api.Data;
using cdpTracker_Api.DTOs;
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
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Find worker by name only — password verified separately with BCrypt
            var worker = await _context.Workers
                .FirstOrDefaultAsync(w => w.Name == request.Name);

            // BCrypt.Verify compares the plain password against the stored hash
            if (worker == null || !BCrypt.Net.BCrypt.Verify(request.Password, worker.PasswordHash))
                return Unauthorized("Invalid username or password.");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, worker.Id.ToString()),
                new Claim(ClaimTypes.Name, worker.Name),
                new Claim(ClaimTypes.Role, worker.Role.ToString()),
                new Claim("kiosko", worker.Kiosko.ToString())
            };

            var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
                ?? _config["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key not configured.");

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(5),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                workerName = worker.Name,
                workerId = worker.Id
            });
        }
    }
}
