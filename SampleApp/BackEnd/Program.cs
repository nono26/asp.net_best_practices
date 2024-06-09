using BackEnd.Infrastructure;
using BackEnd.Logic;
using Microsoft.AspNetCore.ResponseCompression;
using SampleApp.BackEnd.Attributes;
using SampleApp.BackEnd.BackgroundServices;
using SampleApp.BackEnd.BackgroundServices.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SampleApp.BackEnd.Models;
using SampleApp.BackEnd.Mapping;

var builder = WebApplication.CreateBuilder(args);
//https://learn.microsoft.com/fr-fr/dotnet/api/microsoft.aspnetcore.builder.webapplication.createbuilder?view=aspnetcore-8.0
//Initialise une nouvelle instance de la WebApplicationBuilder classe avec des valeurs par défaut préconfigurées.
//La classe WebApplicationBuilder est un générateur d’applications et de services web.
//C'est l'implémentation de l'interface IHostApplicationBuilder: Represents a hosted applications and services builder that helps manage configuration, logging, and lifetime.

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//Configure ApiExplorer à l’aide de Metadata. permet de générere la documentation des APIs en utilisant les métadonnées des points de terminaison.

builder.Services.AddSwaggerGen(c =>
{
    //change the title and the version of the documentation
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "WebAPI Best Practices", Version = "v1" });
    //Added the authorization header to the swagger documentation (allow to add a token to the request)
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\""
    });
    //Added
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddControllers(options =>
{
    options.RespectBrowserAcceptHeader = true; //respect the browser accept header
    options.Filters.Add<GlobalResponseHeaderAttribute>();//add a header to the response for each request #filterAttribut_3
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

builder.Services.AddScoped<ValidationFilterAttribute>();// for the ValidationFilterAttribute

//JWT Bearer
//read the mySettings section from the appsettings.json
var tokenOptionsConfig = builder.Configuration.GetSection("TokenOptions");

builder.Services.Configure<TokenOptions>(tokenOptionsConfig);
var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<TokenOptions>();

var signingConfigurations = new SigningConfigurations(tokenOptions.Secret);
builder.Services.AddSingleton(signingConfigurations);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = tokenOptions.Issuer,
                ValidAudience = tokenOptions.Audience,
                IssuerSigningKey = signingConfigurations.SecurityKey,
                ClockSkew = TimeSpan.Zero
            };
    });

//add automapper 
builder.Services.AddAutoMapper(typeof(DomainToDtoProfile));
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

//Order is important!
app.UseAuthentication();//it's lets your app know who the user is
app.UseAuthorization();// it's ensures that the user is allowed to access the resources

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

