using Gordon360.Authorization;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.MyGordon.Context;
using Gordon360.Models.webSQL.Context;
using Gordon360.Services;
using Gordon360.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Formatting.Compact;
using System;
using System.Collections.Generic;
using System.IO;

const string CorsPolicy = "360UI";

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration, "AzureAd");

    builder.Services.AddControllers(options =>
    {
        options.OutputFormatters.RemoveType<StringOutputFormatter>(); // Return strings as application/json instead of text/plain
        options.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>(); // Return null as 200 Ok null instead of 204 No Content
    }).AddNewtonsoftJson(options => options.UseMemberCasing());

    builder.Services.AddEndpointsApiExplorer();

    var azureConfig = builder.Configuration.GetSection("AzureAd").Get<AzureAdConfig>();

    builder.Services.AddSwaggerGen(c =>
    {
        c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows()
            {
                AuthorizationCode = new OpenApiOAuthFlow()
                {
                    AuthorizationUrl = new Uri($"https://login.microsoftonline.com/{azureConfig.TenantId}/oauth2/v2.0/authorize"),
                    TokenUrl = new Uri($"https://login.microsoftonline.com/{azureConfig.TenantId}/oauth2/v2.0/token"),
                    Scopes = new Dictionary<string, string> {
                {
                    $"{azureConfig.Audience}/access_as_user",
                    "Access 360 as you."
                }
            }
                }
            }
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement() {
            {
                new OpenApiSecurityScheme {
                    Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "oauth2"
                        },
                        Scheme = "oauth2",
                        Name = "oauth2",
                        In = ParameterLocation.Header
                },
                new List<string>()
            }
        });
    });

    builder.Services.AddCors(p => p.AddPolicy(name: CorsPolicy, corsBuilder =>
    {
        corsBuilder.WithOrigins(builder.Configuration.GetValue<string>("AllowedOrigin")).AllowAnyMethod().AllowAnyHeader();
    }));

    builder.Services.AddDbContext<CCTContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("CCT"))
    ).AddDbContext<MyGordonContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("MyGordon"))
    ).AddDbContext<webSQLContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("webSQL"))
    );

    builder.Services.Add360Services();
    builder.Services.AddHostedService<EventCacheRefreshService>();
    builder.Services.AddScoped<ServerUtils, ServerUtils>();

    builder.Services.AddMemoryCache();

    var app = builder.Build();

    // Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.OAuthClientId(azureConfig.ClientId);
        c.OAuthScopes($"{azureConfig.Audience}/access_as_user");
        c.OAuthUsePkce();
    });

    app.UseSerilogRequestLogging();

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
            Path.Combine(builder.Environment.ContentRootPath, "browseable")),
        RequestPath = "/browseable"
    });

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseCors(CorsPolicy);

    app.MapControllers();

    app.Run();

    // Only runs once the app shutsdown
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occurred during startup");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

