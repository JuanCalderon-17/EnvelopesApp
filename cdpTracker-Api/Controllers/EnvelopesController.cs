using Microsoft.AspNetCore.Mvc;
using cdpTracker_Api.Data;
using cdpTracker_Api.Models;
using cdpTracker_Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace cdpTracker_Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EnvelopesController : ControllerBase
	{
		private readonly AppDbContext _context;

		public EnvelopesController(AppDbContext context)
		{
			_context = context;
		}

		[HttpPost]
		public async Task<IActionResult> CreateEnvelope([FromBody] CreateEnvelopeRequest request)
		{
			//validation basic business rules
			if (request.Amount <= 0) return BadRequest("Amount must be greater than zero.");
			if (request.Code.Length != 4) return BadRequest("Code must be 4 digits");

			//verification, does the worker exist in the db
			var workerExist = await _context.Workers.AnyAsync(w => w.Id == request.WorkerId);
			if (!workerExist) return NotFound("Worker not found.");

			//Mapping, this converts dto to database model
			var newEnvelope = new Envelope
			{
				Code = request.Code,
				Amount = request.Amount,
				WorkerId = request.WorkerId,
				RecordedAt = DateTime.UtcNow
			};

			//Persistence, this saves the new envelope to the database
			_context.Envelopes.Add(newEnvelope);
			await _context.SaveChangesAsync();

			return Ok(new { Message = "Envelope created successfully", EnvelopeId = newEnvelope.Id });
		}
	}
}