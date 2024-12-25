using DotNetEnv;
using Server.Models;
using Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
Env.Load();

// Add services to the container.
// Binds MongoDB credentials to MongoDBSettings.cs
builder.Services.Configure<MongoDBSettings>(options =>
{
    options.ConnectionURI = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_URI");
    options.DatabaseName = builder.Configuration.GetSection("MongoDB:DatabaseName").Value;
    options.PlaylistCollectionName = builder.Configuration.GetSection("MongoDB:PlaylistCollectionName").Value;
});

// Registers MongoDBService.cs as a singleton, this means that the same instance will be used throughout the application
builder.Services.AddSingleton<MongoDBService>();


builder.Services.AddControllers();
//System.Console.WriteLine("Connected to Database");

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
