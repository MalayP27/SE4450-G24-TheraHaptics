using System;
using Microsoft.AspNetCore.Mvc;
using Server.Services;
using Server.Models;

namespace Server.Controllers; 

[Controller]
[Route("api/[controller]")]

public class ProductKeyController: Controller {
    private readonly MongoDBService _mongoDBService;

    public ProductKeyController(MongoDBService mongoDBService) {
        _mongoDBService = mongoDBService;
    }

    // [HttpGet("{therapistId}")]
    // public async Task<IActionResult> Get(string therapistId) {
    //     var therapist = await _mongoDBService.GetTherapistAsync(therapistId);
    //     if (therapist == null) {
    //         return NotFound();
    //     }
    //     return Ok(therapist);
    // }

    // [HttpPut("{id}")]
    // public async Task<IActionResult> AddToPlaylist(string id, [FromBody] string movieId) {
    //     await _mongoDBService.AddToPlaylistAsync(id, movieId);
    //     return NoContent();
    // }
}