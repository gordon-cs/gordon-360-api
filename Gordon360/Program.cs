using Gordon360.Models.CCT.Context;
using Gordon360.Models.MyGordon.Context;
using Gordon360.Models.StudentTimesheets.Context;
using Gordon360.Models.webSQL.Context;
using RecIM = Gordon360.Services.RecIM;
using Gordon360.Services;
using Gordon360.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Identity.Web;
using System.IO;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System;

var builder = WebApplication.CreateBuilder(args);

var azureConfig = builder.Configuration.GetSection("AzureAd").Get<AzureAdConfig>();

// Add services to the container.
builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration, "AzureAd");

builder.Services.AddControllers(options =>
{
    options.OutputFormatters.RemoveType<StringOutputFormatter>(); // Return strings as application/json instead of text/plain
    options.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>(); // Return null as 200 Ok null instead of 204 No Content
}).AddNewtonsoftJson(options => options.UseMemberCasing());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
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
        new List < string > ()
    }
});
}
);

string corsPolicy = "360UI";
builder.Services.AddCors(p => p.AddPolicy(name: corsPolicy, corsBuilder =>
{
    corsBuilder.WithOrigins(builder.Configuration.GetValue<string>("AllowedOrigin")).AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddDbContext<CCTContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CCT"))
).AddDbContext<MyGordonContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyGordon"))
).AddDbContext<StudentTimesheetsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("StudentTimesheets"))
).AddDbContext<webSQLContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("webSQL"))
);

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IActivityService, ActivityService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IErrorLogService, ErrorLogService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IAddressesService, AddressesService>();
builder.Services.AddScoped<IMembershipService, MembershipService>();
builder.Services.AddScoped<IMembershipRequestService, MembershipRequestService>();
builder.Services.AddScoped<IAdministratorService, AdministratorService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<ServerUtils, ServerUtils>();
builder.Services.AddHostedService<EventCacheRefreshService>();
builder.Services.AddScoped<RecIM.IActivityService, RecIM.ActivityService>();
builder.Services.AddScoped<RecIM.ISeriesService, RecIM.SeriesService>();
builder.Services.AddScoped<RecIM.IMatchService, RecIM.MatchService>();
builder.Services.AddScoped<RecIM.ITeamService, RecIM.TeamService>();
builder.Services.AddScoped<RecIM.IParticipantService, RecIM.ParticipantService>();
builder.Services.AddScoped<RecIM.ISportService, RecIM.SportService>();
builder.Services.AddScoped<RecIM.IRecIMService, RecIM.RecIMService>();

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

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "browseable")),
    RequestPath = "/browseable"
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors(corsPolicy);

app.MapControllers();

app.Run();
