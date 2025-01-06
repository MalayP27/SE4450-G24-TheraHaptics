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

    private string GenerateTemporaryPassword() {
        const string upperCase = "ABCDEFGHJKLMNOPQRSTUVWXYZ";
        const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string specialChars = "!@#$%^&*()";
        const string allChars = upperCase + lowerCase + digits + specialChars;

        var random = new Random();
        var password = new char[12];

        // Ensure at least one character from each required set
        password[0] = upperCase[random.Next(upperCase.Length)];
        password[1] = lowerCase[random.Next(lowerCase.Length)];
        password[2] = digits[random.Next(digits.Length)];
        password[3] = specialChars[random.Next(specialChars.Length)];

        // Fill the remaining characters with random characters from all sets
        for (int i = 4; i < password.Length; i++) {
            password[i] = allChars[random.Next(allChars.Length)];
        }

        // Shuffle the password to ensure randomness
        return new string(password.OrderBy(x => random.Next()).ToArray());
    }

    private string GenerateSalt() {
        var saltBytes = new byte[64];
        RandomNumberGenerator.Fill(saltBytes);
        return Convert.ToBase64String(saltBytes);
    }

    private string HashPassword(string password, string salt) {
        using (var sha256 = SHA256.Create()) {
            var saltedPassword = password + salt;
            var saltedPasswordBytes = Encoding.UTF8.GetBytes(saltedPassword);
            var hashBytes = sha256.ComputeHash(saltedPasswordBytes);
            return Convert.ToBase64String(hashBytes);
        }
    }

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
        // Validate all parameters are populated
        if (request == null ||
            string.IsNullOrEmpty(request.firstName) ||
            string.IsNullOrEmpty(request.lastName) ||
            string.IsNullOrEmpty(request.emailAddress) ||
            string.IsNullOrEmpty(request.diagnosis)) {
            return BadRequest("All fields are required.");
        }

        // Validate email address
        if (!IsValidEmail(request.emailAddress)) {
            return BadRequest("Invalid email address.");
        }
        
        // Check if email address is already registered
        var existingPatient = await _mongoDBService.GetPatientByEmailAsync(request.emailAddress);
        if (existingPatient != null) {
            return BadRequest("The email address you entered is already associated with an existing account. Please try using a different email address to register patient.");
        }

        var tempPassword = GenerateTemporaryPassword();
        var salt = GenerateSalt();
        var hashedPassword = HashPassword(tempPassword, salt);

        // Creating objects to save into DB
        var objectId = ObjectId.GenerateNewId().ToString();

        var user = new User (
            objectId,
            request.emailAddress,
            salt,
            hashedPassword,
            "patient",
            DateTime.UtcNow,
            true,
            null
        );

        var patient = new Patient(
            objectId,
            request.firstName,
            request.lastName,
            request.emailAddress,
            request.diagnosis
        );

        await _mongoDBService.CreateUserAsync(user);
        await _mongoDBService.CreatePatientAsync(patient);

        // Need to send email to patient with temp password

        // Need to adjust therapist tuple in db to append patient to patient list
        return Ok();
    }
}