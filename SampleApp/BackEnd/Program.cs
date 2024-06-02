using BackeEnd.Infrastructure;
using BackEnd.Logic;

var builder = WebApplication.CreateBuilder(args);
//https://learn.microsoft.com/fr-fr/dotnet/api/microsoft.aspnetcore.builder.webapplication.createbuilder?view=aspnetcore-8.0
//Initialise une nouvelle instance de la WebApplicationBuilder classe avec des valeurs par défaut préconfigurées.
//La classe WebApplicationBuilder est un générateur d’applications et de services web.
//C'est l'implémentation de l'interface IHostApplicationBuilder: Represents a hosted applications and services builder that helps manage configuration, logging, and lifetime.

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//Configure ApiExplorer à l’aide de Metadata. permet de générere la documentation des APIs en utilisant les métadonnées des points de terminaison.

builder.Services.AddSwaggerGen();

builder.Services.AddControllers(); //for the controllers

builder.Services.AddInfrastructure(); // for the Infrastructure services (I/O)

builder.Services.AddLogic(); // for the Logic services (MediatR, etc).

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();// for the Controllers
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

# region "Minimun API"

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/MinAPIweatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("MinAPIGetWeatherForecast")
.WithOpenApi();

#endregion "Minimun API"

app.Run();

