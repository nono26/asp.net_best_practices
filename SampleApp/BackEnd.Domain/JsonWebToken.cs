namespace SampleApp.BackEnd.Domain
{
    public class JsonWebToken
    {
        public string Token { get; init; }
        public long Expiration { get; init; }

        public JsonWebToken(string token, long expiration)
        {
            Token = token;
            Expiration = expiration;
        }

        public bool IsExpired() => DateTime.UtcNow.Ticks > Expiration;
    }
}