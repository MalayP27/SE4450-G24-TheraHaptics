using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.Services;
using Server.Models;
using MongoDB.Bson;
using Microsoft.AspNetCore.Authorization;
using Server.Middleware;

namespace Server.Controllers;

[Controller]
[Route("api/[controller]")]
public class ExerciseProgramController : Controller {
    private readonly MongoDBService _mongoDBService;

    public ExerciseProgramController(MongoDBService mongoDBService) {
        _mongoDBService = mongoDBService;
    }

    // Get all exercise programs
    [Authorize(Policy = "Therapist")]
    [HttpGet("getExerciseProgram/{exerciseId}")]
    public async Task<IActionResult> GetExerciseProgramByIdAsync(string exerciseId) {
        if (string.IsNullOrEmpty(exerciseId)) {
            return BadRequest(new { error = "Exercise ID is required." });
        }

        var exercise = await _mongoDBService.GetExerciseByIdAsync(exerciseId);
        if (exercise == null) {
            return NotFound(new { error = "Exercise not found." });
        }

        return Ok(exercise);
    }

    // Create a new exercise program
    [Authorize(Policy = "TherapistOnly")]
    [HttpPost("createExerciseProgram")]
    public async Task<IActionResult> CreateExerciseProgram([FromBody] ExerciseProgramCreateDto request) {
        if (request == null ||
            string.IsNullOrEmpty(request.Name) ||
            string.IsNullOrEmpty(request.AssignedTo) ||
            request.Exercises == null ||
            request.StartDate == default ||
            request.EndDate == default ||
            string.IsNullOrEmpty(request.DoctorsNotes) ||
            string.IsNullOrEmpty(request.Intensity) ||
            request.EstimatedTime <= 0) {
            return BadRequest(new { error = "All fields are required and must be valid." });
        }

        var exerciseProgram = new ExerciseProgram(
            ObjectId.GenerateNewId().ToString(),
            request.Name,
            request.AssignedTo,
            request.Exercises,
            request.StartDate,
            request.EndDate,
            request.DoctorsNotes,
            request.Intensity,
            request.EstimatedTime
        );

        await _mongoDBService.CreateExerciseProgramAsync(exerciseProgram);

        return Ok(new { message = "Exercise program created successfully.", exerciseProgram });
    }

    // Add an exercise to an exercise program
    [Authorize(Policy = "TherapistOnly")]
    [HttpPost("addExerciseToProgram/{exerciseProgramId}/{exerciseName}")]
    public async Task<IActionResult> AddExerciseToProgram(string exerciseProgramId, string exerciseName) {
        if (string.IsNullOrEmpty(exerciseProgramId) || string.IsNullOrEmpty(exerciseName)) {
            return BadRequest(new { error = "Exercise Program ID and Exercise Name are required." });
        }

        var exerciseProgram = await _mongoDBService.GetExerciseProgramByIdAsync(exerciseProgramId);
        if (exerciseProgram == null) {
            return NotFound(new { error = "Exercise program not found." });
        }

        var exercise = await _mongoDBService.GetExerciseByNameAsync(exerciseName);
        if (exercise == null) {
            return NotFound(new { error = "Exercise not found." });
        }

        exerciseProgram.Exercises.Add(exercise.exerciseId);
        await _mongoDBService.UpdateExerciseProgramAsync(exerciseProgram);

        return Ok(new { message = "Exercise added to program successfully.", exerciseProgram });
    }


    // Remove an exercise from an exercise program
    [Authorize(Policy = "TherapistOnly")]
    [HttpDelete("removeExerciseFromProgram/{exerciseProgramId}/{exerciseId}")]
    public async Task<IActionResult> RemoveExerciseFromProgram(string exerciseProgramId, string exerciseId) {
        if (string.IsNullOrEmpty(exerciseProgramId) || string.IsNullOrEmpty(exerciseId)) {
            return BadRequest(new { error = "Exercise Program ID and Exercise ID are required." });
        }

        var exerciseProgram = await _mongoDBService.GetExerciseProgramByIdAsync(exerciseProgramId);
        if (exerciseProgram == null) {
            return NotFound(new { error = "Exercise program not found." });
        }

        if (!exerciseProgram.Exercises.Remove(exerciseId)) {
            return NotFound(new { error = "Exercise not found in the program." });
        }

        await _mongoDBService.UpdateExerciseProgramAsync(exerciseProgram);

        return Ok(new { message = "Exercise removed from program successfully.", exerciseProgram });
    }

    // Delete an exercise program
    [Authorize(Policy = "TherapistOnly")]
    [HttpDelete("deleteExerciseProgram/{exerciseProgramId}")]
    public async Task<IActionResult> DeleteExerciseProgram(string exerciseProgramId) {
        if (string.IsNullOrEmpty(exerciseProgramId)) {
            return BadRequest(new { error = "Exercise Program ID is required." });
        }

        var exerciseProgram = await _mongoDBService.GetExerciseProgramByIdAsync(exerciseProgramId);
        if (exerciseProgram == null) {
            return NotFound(new { error = "Exercise program not found." });
        }

        await _mongoDBService.DeleteExerciseProgramAsync(exerciseProgramId);

        return NoContent();
    }

    // Update an exercise program
    [Authorize(Policy = "TherapistOnly")]
    [HttpPut("updateExerciseProgram/{exerciseProgramId}")]
    public async Task<IActionResult> UpdateExerciseProgram(string exerciseProgramId, [FromBody] ExerciseProgramUpdateDto request) {
        if (string.IsNullOrEmpty(exerciseProgramId)) {
            return BadRequest(new { error = "Exercise Program ID is required." });
        }

        if (request == null ||
            string.IsNullOrEmpty(request.Name) ||
            string.IsNullOrEmpty(request.AssignedTo) ||
            request.Exercises == null ||
            request.StartDate == default ||
            request.EndDate == default ||
            string.IsNullOrEmpty(request.DoctorsNotes) ||
            string.IsNullOrEmpty(request.Intensity) ||
            request.EstimatedTime <= 0) {
            return BadRequest(new { error = "All fields are required and must be valid." });
        }

        var exerciseProgram = await _mongoDBService.GetExerciseProgramByIdAsync(exerciseProgramId);
        if (exerciseProgram == null) {
            return NotFound(new { error = "Exercise program not found." });
        }

        exerciseProgram.Name = request.Name;
        exerciseProgram.AssignedTo = request.AssignedTo;
        exerciseProgram.Exercises = request.Exercises;
        exerciseProgram.StartDate = request.StartDate;
        exerciseProgram.EndDate = request.EndDate;
        exerciseProgram.DoctorsNotes = request.DoctorsNotes;
        exerciseProgram.Intensity = request.Intensity;
        exerciseProgram.EstimatedTime = request.EstimatedTime;

        await _mongoDBService.UpdateExerciseProgramAsync(exerciseProgram);

        return Ok(new { message = "Exercise program updated successfully.", exerciseProgram });
    }

    // Send an exercise program to a patient
}