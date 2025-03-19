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
public class ExerciseController : Controller {
    private readonly MongoDBService _mongoDBService;

    public ExerciseController(MongoDBService mongoDBService) {
        _mongoDBService = mongoDBService;
    }

    
    //[Authorize(Policy = "TherapistAndPatient")]
    [HttpGet("getExercise/{exerciseId}")]
    public async Task<IActionResult> GetExerciseByIdAsync(string exerciseId) {
        // Validate exercise ID
        if (!ObjectId.TryParse(exerciseId, out ObjectId _)) {
            return BadRequest(new { error = "Invalid exercise ID format." });
        }

        var exercise = await _mongoDBService.GetExerciseByIdAsync(exerciseId);
        if (exercise == null) {
            return NotFound(new { error = "Exercise not found." });
        }

        return Ok(exercise);
    }

    [Authorize(Policy = "TherapistOnly")]
    [HttpPut("updateExercise/{exerciseId}")]
    public async Task<IActionResult> Put(string exerciseId, [FromBody] ExerciseUpdateDto request) {
        //validate exercise ID format
        if (!ObjectId.TryParse(exerciseId, out _)) {
            return BadRequest(new { error = "Invalid exercise ID format." });
        }

        if (request == null ||
            request.TargetReps <= 0 ||
            request.TargetDuration <= 0) {
            return BadRequest(new { error = "All fields are required and must be valid." });
        }

        //fetch the existing exercise using exerciseId
        var exercise = await _mongoDBService.GetExerciseByNameAsync(exerciseId);
        if (exercise == null) {
            return NotFound(new { error = "Exercise not found." });
        }

        exercise.TargetReps = request.TargetReps;
        exercise.TargetDuration = request.TargetDuration;

        //save the updated exercise back to the database
        await _mongoDBService.UpdateExerciseAsync(exercise);

        return Ok(new {
            message = "Exercise information updated successfully.",
            updatedExercise = exercise // Return the updated exercise for confirmation
        });
    }

    //[Authorize(Policy = "TherapistAndPatient")]
    [HttpGet("getAllExercises")]
    public async Task<IActionResult> GetAllExercises() {
        var exercises = await _mongoDBService.GetAllExercisesAsync();
        return Ok(exercises);
    }
}