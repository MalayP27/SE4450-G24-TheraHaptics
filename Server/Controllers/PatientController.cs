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
using Microsoft.AspNetCore.Authorization;
using EmailServiceManager;

namespace Server.Controllers;

[Controller]
[Route("api/[controller]")]
public class PatientController : Controller {
    private readonly MongoDBService _mongoDBService;
    private readonly EmailService _emailService;

    public PatientController(MongoDBService mongoDBService) {
        _mongoDBService = mongoDBService;
        _emailService = new EmailService();
    }

    // Pain Report Email Notification
    private async Task SendPainReportEmail(string therapistId, Patient patient, PainReport painReport) {
        try {
            // Retrieve the therapist using therapistId
            var therapist = await _mongoDBService.GetTherapistByIdAsync(therapistId);
            if (therapist == null) {
                throw new InvalidOperationException($"Therapist with ID {therapistId} not found.");
            }

            string emailSubject = "New Pain Report from Patient";
            string emailBody = $@"
                <p>Dear {therapist.firstName} {therapist.lastName},</p>
                <p>Your patient, {patient.firstName} {patient.lastName}, has reported a new pain.</p>
                <p><strong>Description:</strong> {painReport.Description}</p>
                <p><strong>Pain Level:</strong> {painReport.PainLevel}</p>
                <p><strong>Date Reported:</strong> {painReport.DateReported}</p>
                <p>Please log in to the TheraHaptics system to review the full report and take necessary actions.</p>
                <p>Thank you,</p>
                <p>The TheraHaptics Team</p>
            ";

            _emailService.SendEmail(therapist.emailAddress, emailSubject, emailBody);
        }
        catch (Exception ex) {
            throw new InvalidOperationException($"Failed to send pain report email: {ex.Message}", ex);
        }
    }

    // Patient Registration and Profile Update
    [Authorize(Policy = "PatientOnly")]
    [HttpPut("patientRegistration/{patientId}")]
    public async Task<IActionResult> Put(string patientId, [FromBody] PatientUpdateDto request) {
        // Validate patient ID format
        if (!ObjectId.TryParse(patientId, out _)) {
            return BadRequest(new { error = "Invalid patient ID format." });
        }

        if (request == null ||
            string.IsNullOrEmpty(request.firstName) ||
            string.IsNullOrEmpty(request.lastName) ||
            string.IsNullOrEmpty(request.emailAddress) ||
            string.IsNullOrEmpty(request.phoneNumber) ||
            string.IsNullOrEmpty(request.address) ||
            string.IsNullOrEmpty(request.gender) ||
            request.dateOfBirth == null) {
            return BadRequest(new { error = "All fields are required." });
        }

        // Canonicalize the email address
        request.emailAddress = request.emailAddress.ToLower();

        // Fetch the existing patient using patientId
        var patient = await _mongoDBService.GetPatientByIdAsync(patientId);
        if (patient == null) {
            return NotFound(new { error = "Patient not found." });
        }

        patient.firstName = request.firstName;
        patient.lastName = request.lastName;
        patient.emailAddress = request.emailAddress;
        patient.phoneNumber = request.phoneNumber;
        patient.address = request.address;
        patient.gender = request.gender;
        patient.dateOfBirth = request.dateOfBirth.Value;

        // Save the updated patient back to the database
        await _mongoDBService.UpdatePatientAsync(patient);

        return Ok(new {
            message = "Patient information updated successfully.",
            updatedPatient = patient // Return the updated patient for confirmation
        });
    }

    // Get patient by ID
  //[Authorize(Policy = "TherapistOnly")]
    [HttpGet("getPatient/{patientId}")]
    public async Task<IActionResult> Get(string patientId) {
        // Validate patient ID
        if (!ObjectId.TryParse(patientId, out ObjectId _)) {
            return BadRequest(new { error = "Invalid patient ID format." });
        }

        var patient = await _mongoDBService.GetPatientByIdAsync(patientId);
        if (patient == null) {
            return NotFound(new { error = "Patient not found." });
        }
        return Ok(patient);
    }

    // Get all exercise programs by patient ID
    //[Authorize(Policy = "PatientOnly")]
    [HttpGet("getExerciseProgramsByPatientId/{patientId}")]
    public async Task<IActionResult> GetExerciseProgramsByPatientIdAsync(string patientId) {
        if (string.IsNullOrEmpty(patientId)) {
            return BadRequest(new { error = "Patient ID is required." });
        }

        // Validate patient ID format
        if (!ObjectId.TryParse(patientId, out _)) {
            return BadRequest(new { error = "Invalid patient ID format." });
        }

        // Fetch exercise programs for the given patient ID
        var exercisePrograms = await _mongoDBService.GetExerciseProgramsByPatientIdAsync(patientId);
        if (exercisePrograms == null || !exercisePrograms.Any()) {
            return NotFound(new { error = "No exercise programs found for the specified patient." });
        }

        return Ok(exercisePrograms);
    }

    // Get Pain Reports
    [Authorize (Policy = "TherapistAndPatient")]
    [HttpGet("getPainReports/{patientId}")]
    public async Task<IActionResult> GetPainReports(string patientId) {
        // Validate patient ID format
        if (!ObjectId.TryParse(patientId, out _)) {
            return BadRequest(new { error = "Invalid patient ID format." });
        }

        var patient = await _mongoDBService.GetPatientByIdAsync(patientId);
        if (patient == null) {
            return NotFound(new { error = "Patient not found." });
        }

        var painReports = await _mongoDBService.GetPainReportsByPatientIdAsync(patientId);
        return Ok(painReports);
    }
    
    // Create Pain Report
    //[Authorize(Policy = "PatientOnly")]
    [HttpPost("reportPain/{patientId}")]
    public async Task<IActionResult> Post(string patientId, [FromBody] PainReportCreateDto request) {
        // Validate patient ID format
        if (!ObjectId.TryParse(patientId, out _)) {
            return BadRequest(new { error = "Invalid patient ID format." });
        }

        if (request == null) {
            return BadRequest(new { error = "All fields are required and must be valid." });
        }

        // Fetch the existing patient using patientId
        var patient = await _mongoDBService.GetPatientByIdAsync(patientId);
        if (patient == null) {
            return NotFound(new { error = "Patient not found." });
        }

        // Create a new PainReport object
        var painReport = new PainReport {
            PainReportID = ObjectId.GenerateNewId().ToString(),
            PatientID = patientId,
            Description = request.Description,
            PainLevel = request.PainLevel,
            DateReported = DateTime.Now
        };

        // Save the new pain report to the database
        await _mongoDBService.CreatePainReportAsync(painReport);

        // Send email notification to the therapist
        await SendPainReportEmail(patient.therapistId, patient, painReport);

        return Ok(new {
            message = "Pain report created and sent successfully.",
            painReport
        });
    }

    // Get Progress Reports
    [Authorize (Policy = "TherapistAndPatient")]
    [HttpGet("getProgressReports/{patientId}")]
    public async Task<IActionResult> GetProgressReports(string patientId) {
        // Validate patient ID format
        if (!ObjectId.TryParse(patientId, out ObjectId _)) {
            return BadRequest(new { error = "Invalid patient ID format." });
        }

        var patient = await _mongoDBService.GetPatientByIdAsync(patientId);
        if (patient == null) {
            return NotFound(new { error = "Patient not found." });
        }

        var progressReports = await _mongoDBService.GetProgressReportsByPatientIdAsync(patientId);
        return Ok(progressReports);
    }

    // Create Progress Report
    [Authorize(Policy = "TherapistAndPatient")]
    [HttpPost("createProgressReport/{patientId}")]
    public async Task<IActionResult> CreateProgressReport(string patientId, [FromBody] ProgressReportCreateDto request) {
        // Validate patient ID format
        if (!ObjectId.TryParse(patientId, out ObjectId _)) {
            return BadRequest(new { error = "Invalid patient ID format." });
        }

        if (request == null ||
            string.IsNullOrEmpty(request.Description) ||
            string.IsNullOrEmpty(request.TimeProgression) ||
            string.IsNullOrEmpty(request.RepProgression) ||
            string.IsNullOrEmpty(request.DifficultyImprovement) ||
            request.DateReported == default) {
            return BadRequest(new { error = "All fields are required and must be valid." });
        }

        // Fetch the existing patient using patientId
        var patient = await _mongoDBService.GetPatientByIdAsync(patientId);
        if (patient == null) {
            return NotFound(new { error = "Patient not found." });
        }

        // Create a new ProgressReport object
        var progressReport = new ProgressReport {
            ProgressReportID = ObjectId.GenerateNewId().ToString(),
            PatientID = patientId,
            DateReported = request.DateReported,
            Description = request.Description,
            TimeProgression = request.TimeProgression,
            RepProgression = request.RepProgression,
            DifficultyImprovement = request.DifficultyImprovement
        };

        // Save the new progress report to the database
        await _mongoDBService.CreateProgressReportAsync(progressReport);

        return Ok(new {
            message = "Progress report created successfully.",
            progressReport
        });
    }

    // Get Goals
    [Authorize (Policy = "PatientOnly")]
    [HttpGet("getGoals/{patientId}")]
    public async Task<IActionResult> GetGoals(string patientId) {
        // Validate patient ID format
        if (!ObjectId.TryParse(patientId, out ObjectId _)) {
            return BadRequest(new { error = "Invalid patient ID format." });
        }

        var patient = await _mongoDBService.GetPatientByIdAsync(patientId);
        if (patient == null) {
            return NotFound(new { error = "Patient not found." });
        }

        var goals = await _mongoDBService.GetGoalsByPatientIdAsync(patientId);
        return Ok(goals);
    }

    // Create Patient Goal
    [Authorize(Policy = "PatientOnly")]
    [HttpPost("createPatientGoal/{patientId}")]
    public async Task<IActionResult> CreatePatientGoal(string patientId, [FromBody] GoalCreateDto request) {
        // Validate patient ID format
        if (!ObjectId.TryParse(patientId, out ObjectId _)) {
            return BadRequest(new { error = "Invalid patient ID format." });
        }

        if (request == null ||
            string.IsNullOrEmpty(request.Description)) {
            return BadRequest(new { error = "All fields are required and must be valid." });
        }

        // Fetch the existing patient using patientId
        var patient = await _mongoDBService.GetPatientByIdAsync(patientId);
        if (patient == null) {
            return NotFound(new { error = "Patient not found." });
        }

        // Create a new Goal object
        var goal = new Goal {
            GoalID = ObjectId.GenerateNewId().ToString(),
            PatientID = patientId,
            Description = request.Description
        };

        // Save the new goal to the database
        await _mongoDBService.CreateGoalAsync(goal);

        return Ok(new {
            message = "Goal created successfully.",
            goal
        });
    }

    // Create Session Ended
    [Authorize (Policy = "PatientOnly")]
    [HttpPost("createSessionEnded/{patientId}")]
    public async Task<IActionResult> CreateSessionEnded(string patientId, [FromBody] SessionCreateDto request) {
        // Validate patient ID format
        if (!ObjectId.TryParse(patientId, out ObjectId _)) {
            return BadRequest(new { error = "Invalid patient ID format." });
        }

        if (request == null ||
            request.Date == default ||
            request.Duration == default ||
            string.IsNullOrEmpty(request.Feedback) ||
            request.RepsCompleted <= 0) {
            return BadRequest(new { error = "All fields are required and must be valid." });
        }

        // Fetch the existing patient using patientId
        var patient = await _mongoDBService.GetPatientByIdAsync(patientId);
        if (patient == null) {
            return NotFound(new { error = "Patient not found." });
        }

        // Create a new Session object
        var session = new Session {
            SessionID = ObjectId.GenerateNewId().ToString(),
            PatientID = patientId,
            Date = request.Date,
            Duration = request.Duration,
            Feedback = request.Feedback,
            RepsCompleted = request.RepsCompleted
        };

        // Save the new session to the database
        await _mongoDBService.CreateSessionAsync(session);

        return Ok(new {
            message = "Session ended successfully.",
            session
        });
    }
}