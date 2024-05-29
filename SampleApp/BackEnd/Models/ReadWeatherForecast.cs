using System.ComponentModel.DataAnnotations;
namespace BackEnd.Models;

public record ReadWeatherForecast([Required][Range(1, int.MaxValue)] int Days);