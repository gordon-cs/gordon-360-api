using Gordon360.Database.CCT;
using Gordon360.Database.MyGordon;
using Gordon360.Database.StudentTimesheets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CCTContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Train-CCT"))
).AddDbContext<MyGordonContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Train-MyGordon"))
).AddDbContext<StudentTimesheetsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Train-StudentTimesheets"))
);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
