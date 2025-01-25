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

[Authorize(Policy = "PatientOnly")]
[Controller]
[Route("api/[controller]")]

public class PatientController: Controller {
    private readonly MongoDBService _mongoDBService;

    public PatientController(MongoDBService mongoDBService) {
        _mongoDBService = mongoDBService;
    }

    [HttpGet("dashboard")]
    public IActionResult GetDashboard() {
        return Ok("Patient Dashboard Accessed");
    }

    [HttpPut("patientRegistration/{patientId}")]
    public async Task<IActionResult> Put(string patientId, [FromBody] PatientUpdateDto request) {
        // Validate patient ID format
        if (!ObjectId.TryParse(patientId, out _)) {
            return BadRequest(new { error = "Invalid patient ID format." });
        }

        // Fetch the existing patient using patientId
        var patient = await _mongoDBService.GetPatientByIdAsync(patientId);
        if (patient == null) {
            return NotFound(new { error = "Patient not found." });
        }

        // Pre-fill the existing fields
        patient.firstName = string.IsNullOrEmpty(request.firstName) ? patient.firstName : request.firstName;
        patient.lastName = string.IsNullOrEmpty(request.lastName) ? patient.lastName : request.lastName;
        patient.emailAddress = string.IsNullOrEmpty(request.emailAddress) ? patient.emailAddress : request.emailAddress;
        patient.phoneNumber = string.IsNullOrEmpty(request.phoneNumber) ? patient.phoneNumber : request.phoneNumber;
        patient.address = string.IsNullOrEmpty(request.address) ? patient.address : request.address;
        patient.gender = string.IsNullOrEmpty(request.gender) ? patient.gender : request.gender;
        patient.dateOfBirth = request.dateOfBirth ?? patient.dateOfBirth;

        // Save the updated patient back to the database
        await _mongoDBService.UpdatePatientAsync(patient);

        return Ok(new {
            message = "Patient information updated successfully.",
            updatedPatient = patient // Return the updated patient for confirmation
        });
    }



}