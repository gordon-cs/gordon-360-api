using Gordon360.Database.CCT;
using Gordon360.Database.MyGordon;
using Gordon360.Database.StudentTimesheets;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration, "AzureAd");

builder.Services.AddControllers().AddNewtonsoftJson(options => options.UseMemberCasing());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(p => p.AddPolicy(name: "localhost", builder =>
{
    builder.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddDbContext<CCTContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Train-CCT"))
).AddDbContext<MyGordonContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Train-MyGordon"))
).AddDbContext<StudentTimesheetsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Train-StudentTimesheets"))
);

builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("localhost");

app.MapControllers();

app.Run();
