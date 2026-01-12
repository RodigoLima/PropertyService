namespace PropertyService.Api.Configuration;

public sealed class JwtSettings
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = "IdentityService";
}
