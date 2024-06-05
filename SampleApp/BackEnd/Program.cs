using BackEnd.Infrastructure;
using BackEnd.Logic;
using Microsoft.AspNetCore.ResponseCompression;
using SampleApp.BackEnd.BackgroundServices;
using SampleApp.BackEnd.BackgroundServices.Interfaces;

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

builder.Services.AddControllers(options =>
{
    options.RespectBrowserAcceptHeader = true; //respect the browser accept header
}
).AddXmlSerializerFormatters();// Configuring Content negotiation

builder.Services.AddInfrastructure(); // for the Infrastructure services (I/O)

builder.Services.AddLogic(); // for the Logic services (MediatR, etc).

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;//be careful with this option, check the documentation 
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
}); // for the Response Compression

//we can move them to the Configure method
builder.Services.AddHostedService<DataConsistencyWorkder>(); // for the hosted service
builder.Services.AddScoped<IScopedProcessingService, ScopedProcessingService>();

var app = builder.Build();

app.UseResponseCompression(); // we add the middleware to our request pileline
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error-local-development");
    //app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
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

