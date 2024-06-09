namespace SampleApp.BackEnd.Models;

public record TokenResourcesDto(string AccessToken, string RefreshToken, string expiration);