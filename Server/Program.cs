using DotNetEnv;
using Server.Models;
using Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
DotNetEnv.Env.Load();

// Add services to the container.
// Binds MongoDB credentials to MongoDBSettings.cs
builder.Services.Configure<MongoDBSettings>(options => {
    var connectionURI = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_URI");
    if (string.IsNullOrEmpty(connectionURI)) {
        throw new InvalidOperationException("MongoDB Connection URI is not set in the environment variables.");
    }
    var databaseName = builder.Configuration.GetSection("MongoDB:DatabaseName").Value;
    var playlistCollectionName = builder.Configuration.GetSection("MongoDB:PlaylistCollectionName").Value;
    var productKeyCollectionName = builder.Configuration.GetSection("MongoDB:ProductKeyCollectionName").Value;
    var userCollectionName = builder.Configuration.GetSection("MongoDB:UserCollectionName").Value;
    var therapistCollectionName = builder.Configuration.GetSection("MongoDB:TherapistCollectionName").Value;
    var patientCollectionName = builder.Configuration.GetSection("MongoDB:PatientCollectionName").Value;
    var exerciseCollectionName = builder.Configuration.GetSection("MongoDB:ExerciseCollectionName").Value;
    var exerciseProgramCollectionName = builder.Configuration.GetSection("MongoDB:ExerciseProgramCollectionName").Value;
    var painReportCollectionName = builder.Configuration.GetSection("MongoDB:PainReportCollectionName").Value;

    if (string.IsNullOrEmpty(connectionURI) || string.IsNullOrEmpty(databaseName) || 
        string.IsNullOrEmpty(playlistCollectionName) || string.IsNullOrEmpty(productKeyCollectionName) || 
        string.IsNullOrEmpty(userCollectionName) || string.IsNullOrEmpty(therapistCollectionName) || 
        string.IsNullOrEmpty(patientCollectionName) || string.IsNullOrEmpty(exerciseCollectionName) ||
        string.IsNullOrEmpty(exerciseProgramCollectionName) || string.IsNullOrEmpty(painReportCollectionName)) {
        throw new ArgumentNullException("One or more MongoDB settings are missing.");
    }

    options.ConnectionURI = connectionURI;
    options.DatabaseName = databaseName;
    options.PlaylistCollectionName = playlistCollectionName;
    options.ProductKeyCollectionName = productKeyCollectionName;
    options.UserCollectionName = userCollectionName;
    options.TherapistCollectionName = therapistCollectionName;
    options.PatientCollectionName = patientCollectionName;
    options.ExerciseCollectionName = exerciseCollectionName;
    options.ExerciseProgramCollectionName = exerciseProgramCollectionName;
    options.PainReportCollectionName = painReportCollectionName;
});

// Registers MongoDBService.cs as a singleton, this means that the same instance will be used throughout the application
builder.Services.AddSingleton<MongoDBService>();

// Read the secret key from the environment variable
var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
if (string.IsNullOrEmpty(secretKey)) {
    throw new InvalidOperationException("JWT_SECRET_KEY is not set in the environment variables.");
}

var key = Encoding.ASCII.GetBytes(secretKey);

// // Configure JWT authentication /////////////////////////////////////////////////
// builder.Services.AddAuthentication(options => {
//     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
// })
// .AddJwtBearer(options => {
//     options.RequireHttpsMetadata = true;
//     options.SaveToken = true;
//     options.TokenValidationParameters = new TokenValidationParameters
//     {
//         ValidateIssuerSigningKey = true,
//         IssuerSigningKey = new SymmetricSecurityKey(key),
//         ValidateIssuer = false,
//         ValidateAudience = false
//     };
// });
builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? throw new InvalidOperationException("JWT_SECRET_KEY not set"))
            ),
            ValidateIssuer = false, // Not validating issuer
            ValidateAudience = false, // Not validating audience
            ValidateLifetime = true // Still validate expiration
        };
    });
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("PatientOnly", policy =>
            policy.RequireClaim(ClaimTypes.Role, "patient"));
        options.AddPolicy("TherapistOnly", policy =>
            policy.RequireClaim(ClaimTypes.Role, "therapist"));
        options.AddPolicy("TherapistAndPatient", policy =>
            policy.RequireClaim(ClaimTypes.Role, "therapist", "patient"));
    });

// Add services to the container.


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// For API documentation
// builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
// For api documentation
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }

// Redirects all http traffic to https
// app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();






var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    System.Console.WriteLine("Running an endpoint");
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
