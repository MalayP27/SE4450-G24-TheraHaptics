using System;
using System.Threading.Tasks;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Server.Services;
using Server.Models;
using Server.Middleware;
using MongoDB.Bson;

namespace Server.Controllers; 

[Controller]
[Route("api/[controller]")]

/*
    This controller will handle the following tasks:
        - Create new therapist (handle tuple in both therapist and user collections)
        - Create new patient (handle tuple in both patient and user collections)
        - Login user (handles both therapist and patient)
        - Update user model (handles both therapist and patient)
        - Delete user account (handles both therapist and patient)
*/
public class UserController: Controller {
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

    public UserController(MongoDBService mongoDBService) {
        _mongoDBService = mongoDBService;
    }

    // Use this endpoint when new therapists enter their information in the form and click submit
    // Creates new therapists (assuming the product key entered is valid and not activated yet)
    [HttpPost("therapist")]
    public async Task<IActionResult> Post([FromBody] TherapistPostDto request) {
        // Validate all fieds are populated
        if (request == null ||
            string.IsNullOrEmpty(request.firstName) ||
            string.IsNullOrEmpty(request.lastName) ||
            string.IsNullOrEmpty(request.emailAddress) ||
            string.IsNullOrEmpty(request.password) ||
            string.IsNullOrEmpty(request.productKeyId)) {
                return BadRequest("All fields are required.");
        }
        // Validate email address
        if (!IsValidEmail(request.emailAddress)) {
            return BadRequest("Invalid email address.");
        }
        
        // Check if email address is already registered
        var existingTherapist = await _mongoDBService.GetTherapistByEmailAsync(request.emailAddress);
        if (existingTherapist != null) {
            return BadRequest("The email address you entered is already associated with an existing account. Please try signing in or use a different email address to register.");
        }

        // Validate product key ID
        if (!ObjectId.TryParse(request.productKeyId, out ObjectId _)) {
            return BadRequest("Invalid product key ID format.");
        }
        // Validate password strength
        if (!IsValidPassword(request.password)) {
            return BadRequest("Password must be at least 8 characters long, contain at least one capital letter, one digit, and at least one special character.");
        }

         // Generate salt and hash the password
        var salt = GenerateSalt();
        var hashedPassword = HashPassword(request.password, salt);

        // Check if product key exists and is not activated
        var productKeyObj = await _mongoDBService.GetProductKeyByIdAsync(request.productKeyId);
        if (productKeyObj == null) {
            return BadRequest("Product key not found.");
        }
        if (productKeyObj.isActivated) {
            return BadRequest("Product key already activated.");
        }

        // Creating objects to save into DB
        var objectId = ObjectId.GenerateNewId().ToString();

        var user = new User (
            objectId,
            request.emailAddress,
            salt,
            hashedPassword,
            "therapist",
            DateTime.UtcNow,
            DateTime.UtcNow
        );

        var therapist = new Therapist(
            objectId,
            request.firstName,
            request.lastName,
            request.emailAddress,
            request.productKeyId,
            new List<string>()
        );

        // Activate the product key
        productKeyObj.isActivated = true;
        await _mongoDBService.UpdateProductKeyAsync(productKeyObj);

        await _mongoDBService.CreateUserAsync(user);
        await _mongoDBService.CreateTherapistAsync(therapist);
        return Ok("Therapist created successfully.");
    }
}