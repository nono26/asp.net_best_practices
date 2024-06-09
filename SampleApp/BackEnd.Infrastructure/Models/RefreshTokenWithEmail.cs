using SampleApp.BackEnd.Domain;

namespace SampleApp.BackEnd.Infrastructure.Models;

public class RefreshTokenWithEmail
{
    public string Email { get; set; }
    public RefreshToken RefreshToken { get; set; }
}