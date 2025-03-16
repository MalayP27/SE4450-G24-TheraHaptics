using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.Services;
using Server.Models;
using MongoDB.Bson;
using Microsoft.AspNetCore.Authorization;

namespace Server.Controllers; 


[Controller]
[Route("api/[controller]")]

public class PatientController: Controller {
    private readonly MongoDBService _mongoDBService;

    public PatientController(MongoDBService mongoDBService) {
        _mongoDBService = mongoDBService;
    }


    //Patient Registration and Profile Update
    //[Authorize(Policy = "PatientOnly")]
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
    [Authorize (Policy = "PatientOnly")]
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

    // Report Pain -- //TODO: Email notification to therapist
    [Authorize(Policy = "PatientOnly")]
    [HttpPost("reportPain/{patientId}")]
    public async Task<IActionResult> Post(string patientId, [FromBody] PainReport request) {
        // Validate patient ID format
        if (!ObjectId.TryParse(patientId, out _)) {
            return BadRequest(new { error = "Invalid patient ID format." });
        }

        if (request == null ||
            request.PainLevel <= 0 ||
            string.IsNullOrEmpty(request.Description)) {
            return BadRequest(new { error = "All fields are required and must be valid." });
        }

        // Fetch the existing patient using patientId
        var patient = await _mongoDBService.GetPatientByIdAsync(patientId);
        if (patient == null) {
            return NotFound(new { error = "Patient not found." });
        }

        // Create a new PainReport object
        var painReport = new PainReport(
            ObjectId.GenerateNewId().ToString(),
            patientId,
            request.Description,
            request.PainLevel,
            DateTime.Now
        );

        // Save the new pain report to the database
        await _mongoDBService.CreatePainReportAsync(painReport);

        return Ok(new {
            message = "Pain report created successfully.",
            painReport
        });
    }
    
    // View All Patient's Reports
    [Authorize(Policy = "PatientOnly")]
    [HttpGet("getPainReports/{patientId}")]
    public async Task<IActionResult> GetPainReports(string patientId) {
        // Validate patient ID format
        if (!ObjectId.TryParse(patientId, out ObjectId _)) {
            return BadRequest(new { error = "Invalid patient ID format." });
        }

        var patient = await _mongoDBService.GetPatientByIdAsync(patientId);
        if (patient == null) {
            return NotFound(new { error = "Patient not found." });
        }

        var painReports = await _mongoDBService.GetPainReportsByPatientIdAsync(patientId);
        return Ok(painReports);
    }
    
}