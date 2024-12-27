using System;
using Microsoft.AspNetCore.Mvc;
using Server.Services;
using Server.Models;

namespace Server.Controllers; 

[Controller]
[Route("api/[controller]")]

public class TherapistController: Controller {
    private readonly MongoDBService _mongoDBService;

    public TherapistController(MongoDBService mongoDBService) {
        _mongoDBService = mongoDBService;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Therapist therapist) {
        if (therapist == null ||
            string.IsNullOrEmpty(therapist.firstName) ||
            string.IsNullOrEmpty(therapist.lastName) ||
            string.IsNullOrEmpty(therapist.emailAddress) ||
            string.IsNullOrEmpty(therapist.password) ||
            string.IsNullOrEmpty(therapist.productKey)) {
                return BadRequest("All fields are required.");
        }

        await _mongoDBService.CreateTherapistAsync(therapist);
        return CreatedAtAction(nameof(Get), new { therapistId = therapist.therapistId }, therapist);
    }

    [HttpGet("{therapistId}")]
    public async Task<IActionResult> Get(string therapistId) {
        var therapist = await _mongoDBService.GetTherapistAsync(therapistId);
        if (therapist == null) {
            return NotFound();
        }
        return Ok(therapist);
    }
}