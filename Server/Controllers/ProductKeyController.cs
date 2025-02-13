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

    // Creates product key string of 8 characters
    private string GenerateRandomProductKey() {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        var key = new char[8];
        for (int i = 0; i < key.Length; i++) {
            key[i] = chars[random.Next(chars.Length)];
        }
        return new string(key);
    }

    // Internal API endoint, not to be used by application
    // USE ONLY TO CREATE NEW PRODUCT KEYS
    [HttpPost]
    public async Task<IActionResult> Post() {
        var productKey = new ProductKey {
            isActivated = false,
            productKey = GenerateRandomProductKey()
        };

        await _mongoDBService.CreateProductKeyAsync(productKey);
        return CreatedAtAction(nameof(Get), new { productKey = productKey.productKey }, productKey);
    }
    
    // Use this endpoint when new therapist enters product key from welcome page
    /* 
        If product key returns 200 OK, then save productKeyId from response body internally for when 
        Therapist enters new account information

        If product key returns 400 Bad Request, then show error message to user
    */
    // Checks if product key exists and is not activated
    [HttpGet("{productKey}")]
    public async Task<IActionResult> Get(string productKey) {
        var productKeyObj = await _mongoDBService.GetProductKeyAsync(productKey);
        if (productKeyObj == null) {
            return NotFound(new { error = "Product key not found." });
        }
        if (productKeyObj.isActivated) {
            return Conflict(new { error = "This product key has already been activated." });
        }
        return Ok(new { productKeyId = productKeyObj.productKeyId });
    }
}