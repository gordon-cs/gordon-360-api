using Gordon360.Models.CCT.Context;
using Gordon360.Models.MyGordon.Context;
using Gordon360.Models.StudentTimesheets.Context;
using RecIM = Gordon360.Services.RecIM;
using Gordon360.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Identity.Web;
using System.IO;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration, "AzureAd");

builder.Services.AddControllers(options =>
{
    options.OutputFormatters.RemoveType<StringOutputFormatter>(); // Return strings as application/json instead of text/plain
    options.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>(); // Return null as 200 Ok null instead of 204 No Content
}).AddNewtonsoftJson(options => options.UseMemberCasing());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
);

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IActivityService, ActivityService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IAddressesService, AddressesService>();
builder.Services.AddScoped<RecIM.IActivityService, RecIM.ActivityService>();
builder.Services.AddScoped<RecIM.ISeriesService, RecIM.SeriesService>();
builder.Services.AddScoped<RecIM.IMatchService, RecIM.MatchService>();

builder.Services.AddMemoryCache();
builder.Services.AddMvc().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling
        = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

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
