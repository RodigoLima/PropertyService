namespace PropertyService.Api.Configuration;

public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = "UserService";
    public string Audience { get; set; } = "PropertyService";
}
