
using cdpTracker_Api.Data;
using cdpTracker_Api.DTOs;
using cdpTracker_Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
﻿using Microsoft.EntityFrameworkCore;
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

        [HttpPost]
        public async Task<IActionResult> CreateEnvelope([FromBody] CreateEnvelopeRequest request)
        {
            //validation and its basic rules
            if (request.Amount <= 0)
                return BadRequest("Amount must be greater than zero");

            //extract workerId from the token claims , this is the authenticated user making the request, ensures that the worker can only create envelopes for themselves
            var userIdClaim =  User.FindFirstValue(ClaimTypes.NameIdentifier);
            var workerIdClaim = int.TryParse(userIdClaim, out int workerId) ? workerId : (int?)null; //convert the userIdClaim to an integer

            if (workerIdClaim != request.WorkerId)
            { 
                return Unauthorized("You are not authorized to create an envelope for this worker.Worker not found.");
            }

            // Mapping, convert dto to database model
            var newEnvelope = new Envelope
            {
                Code = request.Code,
                Amount = request.Amount,
                WorkerId = request.WorkerId,
                RecordedAt = DateTime.UtcNow, //server side timestamp for security
            };

            //check if envelope code already exists today, codes are unique per day
            var today = DateTime.UtcNow.Date;
            var envelopeCodeExist = await _context.Envelopes
                .AnyAsync(e => e.Code == newEnvelope.Code && e.RecordedAt.Date == today);
            if (envelopeCodeExist)
            {
                return BadRequest("An envelope with this code already exists today. Please use a unique code.");
            }


            //persistence, save to database(postgres)
            _context.Envelopes.Add(newEnvelope);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Evelope recorded sucessfull!", Id = newEnvelope.Id });


        }
        
        [HttpGet("worker/{workerId}")]//workerId is a parameter in the url, helps to identify which worker's envelopes retrieve
        public async Task<IActionResult> GetWorkerEnvelopes(int workerId)
        {
             //validate  identity token vs Url
            //extrat workerId from the token claims, this is the authenticated user making the request, ensures the worker can only access their own envelopes
            var workerIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(workerIdClaim, out int workerIdValid) || workerIdValid != workerId)
            {
                return Forbid("You are not authorized to access the envelopes for this worker.");
            }


            //check if the worker exist in the database, this is to ensure that we are trying to retrieve envelopes for a valid worker
            //obtaining the envelopes from that specific worker
            var envelopes = await _context.Envelopes
                .Where(e => e.WorkerId == workerId)
                .OrderByDescending(e => e.RecordedAt)
                .Select(e => new EnvelopeResponse //mapping from database model to dto for response, only return the necessary information to the client
                { 
                  Id = e.Id,
                  Code = e.Code,
                  Amount = e.Amount, 
                  RecordedAt = e.RecordedAt,
                  WorkerId = e.WorkerId
                })
                .ToListAsync();


            //check if any envelopes were found for the worker, if not return a not found response
            if (envelopes == null || envelopes.Count == 0)
            {
                return NotFound("No envelopes found for this worker.");
            }

            return Ok(envelopes);
        }
    }
}

