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

//[Authorize(Policy = "PatientOnly")]
[Controller]
[Route("api/[controller]")]

public class PatientController: Controller {
    private readonly MongoDBService _mongoDBService;

    public PatientController(MongoDBService mongoDBService) {
        _mongoDBService = mongoDBService;
    }

    [HttpGet("dashboard")]
    public IActionResult GetDashboard()
    {
        return Ok("Patient Dashboard Accessed");
    }

}