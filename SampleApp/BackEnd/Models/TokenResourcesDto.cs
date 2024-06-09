namespace SampleApp.BackEnd.Models;

public class TokenResourcesDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string expiration { get; set; }
}
