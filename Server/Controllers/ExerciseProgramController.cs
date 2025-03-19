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

        var exerciseProgram = await _mongoDBService.GetExerciseProgramByIdAsync(exerciseId);
        if (exerciseProgram == null) {
            return NotFound(new { error = "Exercise program not found." });
        }

        return Ok(exerciseProgram);
    }

    // Create a new exercise program
    [Authorize(Policy = "TherapistOnly")]
    [HttpPost("createExerciseProgram")]
    public async Task<IActionResult> CreateExerciseProgram([FromBody] ExerciseProgramCreateDto request) {
        if (request == null ||
            string.IsNullOrEmpty(request.Name) ||
            string.IsNullOrEmpty(request.PatientId) ||
            request.Exercises == null ||
            request.StartDate == default ||
            request.EndDate == default ||
            string.IsNullOrEmpty(request.PlanGoals) ||
            string.IsNullOrEmpty(request.Intensity) ||
            request.EstimatedTime <= 0) {
            return BadRequest(new { error = "All fields are required and must be valid." });
        }

        var exercisesInProgram = request.Exercises.Select(e => new ExerciseInProgram(
            e.ExerciseId,
            e.Name,
            e.Instructions,
            e.TargetReps,
            e.TargetDuration,
            e.Intensity
        )).ToList();

        var exerciseProgram = new ExerciseProgram(
            ObjectId.GenerateNewId().ToString(),
            request.Name,
            request.PatientId,
            exercisesInProgram,
            request.StartDate,
            request.EndDate,
            request.PlanGoals,
            request.Intensity,
            request.EstimatedTime
        );

        await _mongoDBService.CreateExerciseProgramAsync(exerciseProgram);

        return Ok(new { message = "Exercise program created successfully.", exerciseProgram });
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
            string.IsNullOrEmpty(request.PatientId) ||
            request.Exercises == null ||
            request.StartDate == default ||
            request.EndDate == default ||
            string.IsNullOrEmpty(request.PlanGoals) ||
            string.IsNullOrEmpty(request.Intensity) ||
            request.EstimatedTime <= 0) {
            return BadRequest(new { error = "All fields are required and must be valid." });
        }

        var exerciseProgram = await _mongoDBService.GetExerciseProgramByIdAsync(exerciseProgramId);
        if (exerciseProgram == null) {
            return NotFound(new { error = "Exercise program not found." });
        }

        var updatedExercises = request.Exercises.Select(e => new ExerciseInProgram(
            e.ExerciseId,
            e.Name,
            e.Instructions,
            e.TargetReps,
            e.TargetDuration,
            e.Intensity
        )).ToList();

        exerciseProgram.Name = request.Name;
        exerciseProgram.PatientId = request.PatientId;
        exerciseProgram.Exercises = updatedExercises;
        exerciseProgram.StartDate = request.StartDate;
        exerciseProgram.EndDate = request.EndDate;
        exerciseProgram.PlanGoals = request.PlanGoals;
        exerciseProgram.Intensity = request.Intensity;
        exerciseProgram.EstimatedTime = request.EstimatedTime;

        await _mongoDBService.UpdateExerciseProgramAsync(exerciseProgram);

        return Ok(new { message = "Exercise program updated successfully.", exerciseProgram });
    }

    // Send an exercise program to a patient
}