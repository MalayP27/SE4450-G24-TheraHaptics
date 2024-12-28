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
    
    // Checks if product key exists and is not activated
    [HttpGet("{productKey}")]
    public async Task<IActionResult> Get(string productKey) {
        var productKeyObj = await _mongoDBService.GetProductKeyAsync(productKey);
        if (productKeyObj == null) {
            return BadRequest("Product key not found.");
        }
        if (productKeyObj.isActivated) {
            return BadRequest("This product key has already been activated.");
        }
        return Ok(productKeyObj);
    }

    // Activates product key
    [HttpPut("{productKey}")]
    public async Task<IActionResult> Put(string productKey) {
        var productKeyObj = await _mongoDBService.GetProductKeyAsync(productKey);
        if (productKeyObj == null) {
            return BadRequest("Something went wrong. Sorry!");
        }

        productKeyObj.isActivated = true;
        await _mongoDBService.UpdateProductKeyAsync(productKeyObj);

        return NoContent();
    }
}