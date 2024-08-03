using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace Gordon360.Options;

public sealed record BonAppetitOptions
{
    public const string BonAppetit = "BonAppetit";

    [Required]
    public required string IssuerID { get; set; }
    [Required]
    public required string ApplicationID { get; set; }
    [Required]
    public required string Secret { get; set; }
}

[OptionsValidator]
public partial class ValidateBonAppetitOptions : IValidateOptions<BonAppetitOptions> { }
