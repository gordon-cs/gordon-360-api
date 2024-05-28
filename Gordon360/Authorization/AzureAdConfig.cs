namespace Gordon360.Authorization;

public record AzureAdConfig
{
    public string Instance { get; init; }
    public string ClientId { get; init; }
    public string TenantId { get; init; }
    public string Audience { get; init; }
}