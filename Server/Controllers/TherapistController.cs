using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.Services;
using Server.Models;
using Server.Middleware;
using MongoDB.Bson;

namespace Server.Controllers; 

[Controller]
[Route("api/[controller]")]

public class TherapistController: Controller {
    private readonly MongoDBService _mongoDBService;

    public TherapistController(MongoDBService mongoDBService) {
        _mongoDBService = mongoDBService;
    }

    // Use when Therapist wants to view their information
    [HttpGet("{therapistId}")]
    public async Task<IActionResult> Get(string therapistId) {
        // Validate therapist ID
        if (!ObjectId.TryParse(therapistId, out ObjectId _)) {
            return BadRequest("Invalid therapist ID format.");
        }
        
        var therapist = await _mongoDBService.GetTherapistByIdAsync(therapistId);
        if (therapist == null) {
            return NotFound();
        }
        return Ok(therapist);
    }

    // Use when Therapist wants to update their information
    // Modifies Therapist information
    // [HttpPut("{therapistId}")]
    // public async Task<IActionResult> Put(string therapistId, [FromBody] Therapist therapist) {
    //     // Validate therapist ID
    //     if (!ObjectId.TryParse(therapistId, out _)) {
    //         return BadRequest("Invalid therapist ID format.");
    //     }
    //     // Validate all fields are populated
    //     if (therapist == null ||
    //         string.IsNullOrEmpty(therapist.firstName) ||
    //         string.IsNullOrEmpty(therapist.lastName) ||
    //         string.IsNullOrEmpty(therapist.emailAddress) ||
    //         string.IsNullOrEmpty(therapist.productKeyId)) {
    //             return BadRequest("All fields are required.");
    //     }
    //     // Validate email address
    //     if (!IsValidEmail(therapist.emailAddress)) {
    //         return BadRequest("Invalid email address.");
    //     }

    //     therapist.therapistId = therapistId;

    //     await _mongoDBService.UpdateTherapistAsync(therapist);
    //     return Ok(therapist);
    // }

    // Use when Therapist wants to delete their account
    // Deletes Therapist account
    // [HttpDelete("{therapistId}")]
    // public async Task<IActionResult> Delete(string therapistId) {
    //     // Validate therapist ID format
    //     if (!ObjectId.TryParse(therapistId, out _)) {
    //         return BadRequest("Invalid therapist ID format.");
    //     }

    //     var therapist = await _mongoDBService.GetTherapistByIdAsync(therapistId);
    //     if (therapist == null) {
    //         return NotFound();
    //     }

    //     await _mongoDBService.DeleteTherapistAsync(therapistId);
    //     return NoContent();
    // }

    // Use when Therapist wants to add a new patient
    [HttpPost("newPatient")]
    public async Task<IActionResult> Post([FromBody] PatientPostDto request) {
        //Stil needs to be implemented
        return Ok();
    }
}