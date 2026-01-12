namespace PropertyService.Api.Configuration;

public class JwtSettings
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = "IdentityService";
    public string Audience { get; set; } = "api";
    public string[]? ValidAudiences { get; set; }
}
