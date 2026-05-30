namespace ErpFactory.Api.Services;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "ErpFactory.Api";
    public string Audience { get; set; } = "ErpFactory.Client";
    public string Key { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 60;
}
