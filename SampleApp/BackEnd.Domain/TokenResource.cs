using Sample.BackEnd.Domain;

namespace SampleApp.BackEnd.Domain;

public class TokenResource : JsonWebToken
{
    public RefreshToken RefreshToken { get; set; }

    public TokenResource(string accessToken, long expiration, RefreshToken refreshToken) : base(accessToken, expiration)
    {
        Token = accessToken;
        RefreshToken = refreshToken;
        Expiration = expiration;
    }
}

public class NullTokenResource : TokenResource
{
    public NullTokenResource() : base(string.Empty, 0, null)
    {
    }
}
