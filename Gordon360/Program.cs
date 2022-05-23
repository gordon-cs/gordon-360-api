using Gordon360.Models.CCT.Context;
using Gordon360.Models.MyGordon.Context;
using Gordon360.Models.StudentTimesheets.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Identity.Web;
using System.IO;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration, "AzureAd");

builder.Services.AddControllers(options =>
{
    // When a route returns a response with no content, encode the response as 200 OK instead of 204 No Content
    // This allows us to return null and, more importantly, an empty list.
    // That way empty lists can be treated the same way as contentful lists on the front end.
    var noContentFormatter = options.OutputFormatters.OfType<HttpNoContentOutputFormatter>().FirstOrDefault();
    if (noContentFormatter != null)
    {
        noContentFormatter.TreatNullValueAsNoContent = false;
    }
}).AddNewtonsoftJson(options => options.UseMemberCasing());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(p => p.AddPolicy(name: "localhost", builder =>
{
    builder.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddDbContext<CCTContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CCT"))
).AddDbContext<MyGordonContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyGordon"))
).AddDbContext<StudentTimesheetsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("StudentTimesheets"))
);

builder.Services.AddMemoryCache();

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

app.UseCors("localhost");

app.MapControllers();

app.Run();
