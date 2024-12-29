using System;
using Microsoft.AspNetCore.Mvc;
using Server.Services;
using Server.Models;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Server.Controllers; 

[Controller]
[Route("api/[controller]")]

public class TherapistController: Controller {
    private readonly MongoDBService _mongoDBService;

    private bool IsValidEmail(string email) {
        if (string.IsNullOrWhiteSpace(email)) {
            return false;
        }

        try {
            // Use Regex to check if the email is valid
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(email);
        }
        catch {
            return false;
        }
    }

    private bool IsValidPassword(string password) {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        // Use Regex to check if the password is valid
        var passwordRegex = new Regex(@"^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*(),.?\:{}|<>]).{8,}$");
        return passwordRegex.IsMatch(password);
    }

    public TherapistController(MongoDBService mongoDBService) {
        _mongoDBService = mongoDBService;
    }

    // Use this endpoint when new therapists enter their information in the form and click submit
    // Creates new therapists (assuming the product key entered is valid and not activated yet)
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Therapist therapist) {
        // Validate all fieds are populated
        if (therapist == null ||
            string.IsNullOrEmpty(therapist.firstName) ||
            string.IsNullOrEmpty(therapist.lastName) ||
            string.IsNullOrEmpty(therapist.emailAddress) ||
            string.IsNullOrEmpty(therapist.password) ||
            string.IsNullOrEmpty(therapist.productKeyId)) {
                return BadRequest("All fields are required.");
        }
        // Validate email address
        if (!IsValidEmail(therapist.emailAddress)) {
            return BadRequest("Invalid email address.");
        }

        // Check if email address is already registered
        var existingTherapist = await _mongoDBService.GetTherapistByEmailAsync(therapist.emailAddress);
        if (existingTherapist != null) {
            return BadRequest("The email address you entered is already associated with an existing account. Please try signing in or use a different email address to register.");
        }
        
        // Validate product key ID
        if (!ObjectId.TryParse(therapist.productKeyId, out _)) {
            return BadRequest("Invalid product key ID format.");
        }
        // Validate password strength
        if (!IsValidPassword(therapist.password)) {
            return BadRequest("Password must be at least 8 characters long, contain at least one capital letter, one digit, and at least one special character.");
        }
        // Check if product key exists and is not activated
        var productKeyObj = await _mongoDBService.GetProductKeyByIdAsync(therapist.productKeyId);
        if (productKeyObj == null) {
            return BadRequest("Product key not found.");
        }
        if (productKeyObj.isActivated) {
            return BadRequest("Product key already activated.");
        }

        // Activate the product key
        productKeyObj.isActivated = true;
        await _mongoDBService.UpdateProductKeyAsync(productKeyObj);

        await _mongoDBService.CreateTherapistAsync(therapist);
        return CreatedAtAction(nameof(Get), new { therapistId = therapist.therapistId }, therapist);
    }

    [HttpGet("{therapistId}")]
    public async Task<IActionResult> Get(string therapistId) {
        // Validate therapist ID
        if (!ObjectId.TryParse(therapistId, out _)) {
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
    [HttpPut("{therapistId}")]
    public async Task<IActionResult> Put(string therapistId, [FromBody] Therapist therapist) {
        // Validate therapist ID
        if (!ObjectId.TryParse(therapistId, out _)) {
            return BadRequest("Invalid therapist ID format.");
        }
        // Validate all fields are populated
        if (therapist == null ||
            string.IsNullOrEmpty(therapist.firstName) ||
            string.IsNullOrEmpty(therapist.lastName) ||
            string.IsNullOrEmpty(therapist.emailAddress) ||
            string.IsNullOrEmpty(therapist.password) ||
            string.IsNullOrEmpty(therapist.productKeyId)) {
                return BadRequest("All fields are required.");
        }
        // Validate email address
        if (!IsValidEmail(therapist.emailAddress)) {
            return BadRequest("Invalid email address.");
        }
        // Validate password strength
        if (!IsValidPassword(therapist.password)) {
            return BadRequest("Password must be at least 8 characters long, contain at least one capital letter, one digit, and at least one special character.");
        }

        therapist.therapistId = therapistId;

        await _mongoDBService.UpdateTherapistAsync(therapist);
        return Ok(therapist);
    }

    // Use when Therapist wants to delete their account
    // Deletes Therapist account
    [HttpDelete("{therapistId}")]
    public async Task<IActionResult> Delete(string therapistId) {
        // Validate therapist ID format
        if (!ObjectId.TryParse(therapistId, out _)) {
            return BadRequest("Invalid therapist ID format.");
        }

        var therapist = await _mongoDBService.GetTherapistByIdAsync(therapistId);
        if (therapist == null) {
            return NotFound();
        }

        await _mongoDBService.DeleteTherapistAsync(therapistId);
        return NoContent();
    }
}