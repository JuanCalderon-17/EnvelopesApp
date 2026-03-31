
﻿using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using cdpTracker_Api.Data;
using cdpTracker_Api.Models;
using cdpTracker_Api.DTOs;

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
            //validation and its basic rules
            if (request.Amount <= 0)
                return BadRequest("Amount must be greater than zero");

            // Now verification follows, does the worker exist
            var workerExists = await _context.Workers.AnyAsync(w => w.Id == request.WorkerId);
            if (!workerExists)
            {
                return NotFound("Worker not found.");
            }

            // Mapping, convert dto to database model
            var newEnvelope = new Envelope
            {
                Code = request.Code,
                Amount = request.Amount,
                WorkerId = request.WorkerId,
                RecordedAt = DateTime.UtcNow, //server side timestamp for security
            };

            //persistence, save to database(postgres)
            _context.Envelopes.Add(newEnvelope);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Evelope recorder sucessfull!", Id = newEnvelope.Id });


        }

        [HttpGet("worker/{workerId}")]//worker id es el parametro que se va a pasar en la url para obtener los sobres de un trabajador especifico para autorizar
        public async Task<IActionResult> GetWorkerEnvelopes(int workerId)
        {
            //verificar que el trabajador exista
            var workerExists = await _context.Workers.AnyAsync(w => w.Id == workerId);
            if (!workerExists)
            {
                return NotFound("Worker not found.");
            }
            //obtener los sobres del trabajador especifico
            var envelopes = await _context.Envelopes
                .Where(e => e.WorkerId == workerId)
                .OrderByDescending(e => e.RecordedAt)
                .ToListAsync();

            if (envelopes == null || envelopes.Count == 0)
            {
                return NotFound("No envelopes found for this worker.");
            }

            return Ok(envelopes);
        }
    }
}

