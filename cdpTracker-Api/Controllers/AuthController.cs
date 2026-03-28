using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using cdpTracker_Api.Data;
using cdpTracker_Api.DTOs;

namespace cdpTracker_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AuthController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            //Find the worker by name using LINQ
            var worker = _context.Workers.FirstOrDefault(w => w.Name == request.Name);

            //Check if worker exists and password matches
            if (worker == null || worker.PasswordHash != request.Password)
            {
                return Unauthorized("Invalid username or password.");
            }

            // If authentication is successful, and return the worker's information
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
