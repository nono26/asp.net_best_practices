namespace SampleApp.BackEnd.Domain;

public class TokenResource
{
    public string AccessToken { get; init; }
    //public string RefreshToken { get; set; }
    public long Expiration { get; init; }

    public TokenResource(string accessToken, long expiration)
    {
        AccessToken = accessToken;
        //RefreshToken = refreshToken;
        Expiration = expiration;
    }
}

public class NullTokenResource : TokenResource
{
    public NullTokenResource() : base(string.Empty, 0)
    {
    }
}
