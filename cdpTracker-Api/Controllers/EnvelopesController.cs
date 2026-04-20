
using cdpTracker_Api.Data;
using cdpTracker_Api.DTOs;
using cdpTracker_Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace cdpTracker_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EnvelopesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public EnvelopesController(AppDbContext context)
        {
            _context = context;
        }

        // GET all envelopes from every worker — any authenticated user can see all
        [HttpGet]
        public async Task<IActionResult> GetAllEnvelopes()
        {
            var envelopes = await _context.Envelopes
                .Include(e => e.Worker)
                .OrderByDescending(e => e.RecordedAt)
                .Select(e => new EnvelopeResponse
                {
                    Id = e.Id,
                    Code = e.Code,
                    Amount = e.Amount,
                    RecordedAt = e.RecordedAt,
                    WorkerId = e.WorkerId,
                    WorkerName = e.Worker.Name,
                    Kiosko = e.Worker.Kiosko.ToString()
                })
                .ToListAsync();

            return Ok(envelopes);
        }

        // GET envelopes for a specific worker
        [HttpGet("worker/{workerId}")]
        public async Task<IActionResult> GetWorkerEnvelopes(int workerId)
        {
            var workerIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(workerIdClaim, out int workerIdValid) || workerIdValid != workerId)
                return Forbid();

            var envelopes = await _context.Envelopes
                .Include(e => e.Worker)
                .Where(e => e.WorkerId == workerId)
                .OrderByDescending(e => e.RecordedAt)
                .Select(e => new EnvelopeResponse
                {
                    Id = e.Id,
                    Code = e.Code,
                    Amount = e.Amount,
                    RecordedAt = e.RecordedAt,
                    WorkerId = e.WorkerId,
                    WorkerName = e.Worker.Name,
                    Kiosko = e.Worker.Kiosko.ToString()
                })
                .ToListAsync();

            if (envelopes.Count == 0)
                return NotFound("No envelopes found for this worker.");

            return Ok(envelopes);
        }

        // POST — create a new envelope
        [HttpPost]
        public async Task<IActionResult> CreateEnvelope([FromBody] CreateEnvelopeRequest request)
        {
            if (request.Amount <= 0)
                return BadRequest("Amount must be greater than zero");

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var workerIdClaim = int.TryParse(userIdClaim, out int workerId) ? workerId : (int?)null;

            if (workerIdClaim != request.WorkerId)
                return Unauthorized("You are not authorized to create an envelope for this worker.");

            var today = DateTime.UtcNow.Date;
            var envelopeCodeExist = await _context.Envelopes
                .AnyAsync(e => e.Code == request.Code && e.RecordedAt.Date == today);
            if (envelopeCodeExist)
                return BadRequest("An envelope with this code already exists today. Please use a unique code.");

            var newEnvelope = new Envelope
            {
                Code = request.Code,
                Amount = request.Amount,
                WorkerId = request.WorkerId,
                RecordedAt = DateTime.UtcNow,
            };

            _context.Envelopes.Add(newEnvelope);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Envelope recorded successfully!", Id = newEnvelope.Id });
        }

        // PUT — edit code and amount of an existing envelope
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEnvelope(int id, [FromBody] UpdateEnvelopeRequest request)
        {
            var envelope = await _context.Envelopes.FindAsync(id);
            if (envelope == null)
                return NotFound("Envelope not found.");

            // Unique code per day, excluding this same envelope
            var today = DateTime.UtcNow.Date;
            var codeExists = await _context.Envelopes
                .AnyAsync(e => e.Code == request.Code && e.RecordedAt.Date == today && e.Id != id);
            if (codeExists)
                return BadRequest("An envelope with this code already exists today.");

            envelope.Code = request.Code;
            envelope.Amount = request.Amount;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Envelope updated successfully." });
        }

        // DELETE — remove an envelope by id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnvelope(int id)
        {
            var envelope = await _context.Envelopes.FindAsync(id);
            if (envelope == null)
                return NotFound("Envelope not found.");

            _context.Envelopes.Remove(envelope);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Envelope deleted successfully." });
        }
    }
}
