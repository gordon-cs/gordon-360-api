using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace Gordon360.Options;

public sealed record AzureAdOptions
{
    public const string AzureAd = "AzureAd";

    [Required]
    public required string Instance {  get; set; }
    [Required]
    public required string ClientId {  get; set; }
    [Required]
    public required string TenantId {  get; set; }
    [Required]
    public required string Audience {  get; set; }
}

[OptionsValidator]
public partial class ValidateAzureAdOptions : IValidateOptions<AzureAdOptions> { }

