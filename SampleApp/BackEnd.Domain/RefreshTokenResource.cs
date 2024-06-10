namespace SampleApp.BackEnd.Domain;

public class RefreshToken : JsonWebToken
{
    public RefreshToken(string token, long expiration) : base(token, expiration)
    {
    }
}