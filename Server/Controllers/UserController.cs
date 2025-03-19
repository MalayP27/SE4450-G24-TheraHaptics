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
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using EmailServiceManager;

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
    private readonly EmailService _emailService;

    //Input Validation
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

    // private string GenerateJwtToken(User user) {
    //     var tokenHandler = new JwtSecurityTokenHandler();
    //     var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");// Use a secure key
    //     if (string.IsNullOrEmpty(secretKey)) {
    //         throw new InvalidOperationException("JWT_SECRET_KEY is not set in the environment variables.");
    //     }
    //     var key = Encoding.ASCII.GetBytes(secretKey);
    //     var tokenDescriptor = new SecurityTokenDescriptor {
    //         Subject = new ClaimsIdentity(new[] {
    //             new Claim(ClaimTypes.NameIdentifier, user.userId ?? string.Empty),
    //             new Claim(ClaimTypes.Email, user.emailAddress ?? string.Empty),
    //             new Claim(ClaimTypes.Role, user.role ?? string.Empty)
    //         }),
    //         Expires = DateTime.UtcNow.AddDays(7),
    //         SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    //     };
    //     var token = tokenHandler.CreateToken(tokenDescriptor);
    //     return tokenHandler.WriteToken(token);
    // }

    private string GenerateJwtToken(User user)
    {
        // Retrieve the secret key from environment variables
        var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("JWT_SECRET_KEY is not set in the environment variables.");
        }

        // Convert the key to bytes
        var key = Encoding.ASCII.GetBytes(secretKey);

        // Define the token's claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.userId ?? string.Empty), // User ID
            new Claim(ClaimTypes.Email, user.emailAddress ?? string.Empty),    // Email Address
            new Claim(ClaimTypes.Role, user.role ?? string.Empty)             // Role (Patient or Therapist)
        };

        // Configure token descriptor
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims), // Attach claims
            Expires = DateTime.UtcNow.AddDays(7), // Token expiration
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature) // Signing credentials
        };

        // Generate the token
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        // Return the token as a string
        return tokenHandler.WriteToken(token);
    }

    private void SendForgotPasswordEmail(string emailAddress, string tempPassword)
    {
        try
        {
            string emailSubject = "Reset Your TheraHaptics Password";
            string emailBody = $@"
                <p>Dear User,</p>
                <p>We received a request to reset your password for your TheraHaptics account.</p>
                <p>Your temporary password is: <strong>{tempPassword}</strong></p>
                <p>Please log in with this temporary password and change it immediately to secure your account.</p>
                <p>If you did not request a password reset, please contact our support team immediately.</p>
                <p>Thank you,</p>
                <p>The TheraHaptics Team</p>
            ";

            _emailService.SendEmail(emailAddress, emailSubject, emailBody);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to send forgot password email to {emailAddress}: {ex.Message}", ex);
        }
    }

    public UserController(MongoDBService mongoDBService) {
        _mongoDBService = mongoDBService;
        _emailService = new EmailService();
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
                return BadRequest(new { error = "All fields are required." });
        }

        // Canonicalize the email address
        request.emailAddress = request.emailAddress.ToLower();

        // Validate email address
        if (!IsValidEmail(request.emailAddress)) {
            return BadRequest(new { error = "Invalid email address."});
        }
        
        // Check if email address is already registered
        var existingTherapist = await _mongoDBService.GetTherapistByEmailAsync(request.emailAddress);
        if (existingTherapist != null) {
            return Conflict(new { error = "The email address is already in use. Please sign in or use a different email." });
        }

        // Validate product key ID
        if (!ObjectId.TryParse(request.productKeyId, out ObjectId _)) {
            return BadRequest(new { error = "Invalid product key ID format." });
        }
        // Validate password strength
        if (!IsValidPassword(request.password)) {
            return BadRequest(new { error = "Password must be at least 8 characters long, contain at least one capital letter, one digit, and one special character." });
        }

         // Generate salt and hash the password
        var salt = GenerateSalt();
        var hashedPassword = HashPassword(request.password, salt);

        // Check if product key exists and is not activated
        var productKeyObj = await _mongoDBService.GetProductKeyByIdAsync(request.productKeyId);
        if (productKeyObj == null) {
            return NotFound(new { error = "Product key not found."});
        }
        if (productKeyObj.isActivated) {
            return Conflict(new { error = "Product key has already been activated." });
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
            false,
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

        // Generate JWT token
        var token = GenerateJwtToken(user);

        // Set the token in a cookie
        Response.Cookies.Append("jwt", token, new CookieOptions {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });
        return Ok(therapist);
    }

    // When user's try to login they will use this endpoint
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request) {
        // Validate input
        if (request == null ||
            string.IsNullOrEmpty(request.emailAddress) ||
            string.IsNullOrEmpty(request.password)) {
            return BadRequest(new { error = "All fields are required." });
        }

        // Canonicalize the email address
        request.emailAddress = request.emailAddress.ToLower();

        // Retrieve the user from the database
        var user = await _mongoDBService.GetUserByEmailAsync(request.emailAddress);
        if (user == null) {
            return NotFound(new { error = "User not found. Please check your email or sign up." });
        }

        // Verify the password
        var computeHashedPassword = HashPassword(request.password, user.passwordSalt);
        if (computeHashedPassword != user.passwordHash) {
            return Unauthorized(new { error = "Invalid email or password." });
        }

        // Update lastLoggedIn field
        user.lastLoggedIn = DateTime.UtcNow;
        await _mongoDBService.UpdateUserAsync(user);

        // Retrieve the therapistId if the user is a therapist
        string? therapistId = null;
        if (user.role == "therapist") {
            var therapist = await _mongoDBService.GetTherapistByEmailAsync(request.emailAddress);
            if (therapist != null) {
                therapistId = therapist.therapistId;
            }
        }

        // Retrieve the patientId if the user is a patient
        string? patientId = null;
        if (user.role == "patient") {
            var patient = await _mongoDBService.GetPatientByEmailAsync(request.emailAddress);
            if (patient != null) {
                patientId = patient.patientId;
            }
        }

        // Generate JWT token
        var token = GenerateJwtToken(user);

        // Set the token in a secure, HTTP-only cookie
        Response.Cookies.Append("jwt", token, new CookieOptions {
            HttpOnly = true,
            Secure = true, // Ensure Secure flag is set for HTTPS
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        // Return the JWT token as part of the response
        return Ok(new { Token = token, Role = user.role, IsTemporaryPassword = user.isTemporaryPassword, TherapistId = therapistId, PatientId = patientId });
    }

    // When users log out, this endpoint deletes the cookies
    [HttpPost("logout")]
    public IActionResult Logout() {
        // Clear the JWT token from the cookie
        Response.Cookies.Delete("jwt");

        return NoContent();
    }


    // When a user isn't authenticated yet, they will use this endpoint to reset password.
    [HttpPost("forgotPassword")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request) {
        var emailAddress = request.emailAddress; // Extract email from DTO

        // Validate email input
        if (string.IsNullOrEmpty(emailAddress) || !IsValidEmail(emailAddress)) {
            return BadRequest(new { error = "Invalid email address format." });
        }

        // Canonicalize the email address
        request.emailAddress = request.emailAddress.ToLower();

        // Check if user exists
        var user = await _mongoDBService.GetUserByEmailAsync(emailAddress);
        if (user == null) {
            return NotFound(new { error = "Email address not associated with any account." });
        }

        // Generate a temporary password
        var tempPassword = GenerateTemporaryPassword();
        var salt = GenerateSalt();
        var hashedPassword = HashPassword(tempPassword, salt);

        // Update the user's password and set isTemporaryPassword flag
        user.passwordSalt = salt;
        user.passwordHash = hashedPassword;
        user.isTemporaryPassword = true;
        await _mongoDBService.UpdateUserAsync(user);

        try {
            SendForgotPasswordEmail(emailAddress, tempPassword);
        }
        catch {
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Failed to send email. Please try again later." });
        }

        return Ok(new { message = "A temporary password has been sent to your email address." });
    }

    // When user enters a new password
    [HttpPut("changePassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request) {
        // Validate input
        if (request == null ||
            string.IsNullOrEmpty(request.emailAddress) ||
            string.IsNullOrEmpty(request.tempPassword) ||
            string.IsNullOrEmpty(request.newPassword)) {
            return BadRequest(new { error = "All fields are required." });
        }

        // Canonicalize the email address
        request.emailAddress = request.emailAddress.ToLower();

        // Retrieve the user from the database
        var user = await _mongoDBService.GetUserByEmailAsync(request.emailAddress);
        if (user == null) {
            return NotFound (new { error = "User not found. Please check your email or sign up." });
        }

        // Verify the temp password
        var computeHashedPassword = HashPassword(request.tempPassword, user.passwordSalt);
        if (computeHashedPassword != user.passwordHash) {
            return Unauthorized(new { error = "Invalid email or password." });
        }

        // Validate password strength
        if (!IsValidPassword(request.newPassword)) {
            return BadRequest(new { error = "Password must be at least 8 characters long, contain at least one capital letter, one digit, and one special character." });
        }

         // Generate salt and hash the password
        var salt = GenerateSalt();
        var hashedPassword = HashPassword(request.newPassword, salt);

        // Update lastLoggedIn field
        user.lastLoggedIn = DateTime.UtcNow;

        // Update the user's password and set isTemporaryPassword flag
        user.passwordSalt = salt;
        user.passwordHash = hashedPassword;
        user.isTemporaryPassword = false;

        await _mongoDBService.UpdateUserAsync(user);

        // Generate JWT token
        var token = GenerateJwtToken(user);

        // Set the token in a secure, HTTP-only cookie
        Response.Cookies.Append("jwt", token, new CookieOptions {
            HttpOnly = true,
            Secure = true, // Ensure Secure flag is set for HTTPS
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        // Return the JWT token as part of the response
        return Ok(new { Token = token, Role = user.role, IsTemporaryPassword = user.isTemporaryPassword });
    }
}