using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Gordon360.Options;

public static class OptionsExtensions
{
    public static IServiceCollection Add360Options(this IServiceCollection services)
    {
        services.AddSingleton<IValidateOptions<BonAppetitOptions>, ValidateBonAppetitOptions>();
        services.AddOptions<BonAppetitOptions>()
            .BindConfiguration(BonAppetitOptions.BonAppetit)
            .ValidateOnStart();

        return services;
    }
}
