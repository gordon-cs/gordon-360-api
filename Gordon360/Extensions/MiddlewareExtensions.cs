
using Gordon360.Utilities.Logger;
using Microsoft.AspNetCore.Builder;
// See https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/extensibility?view=aspnetcore-6.0 
// for background on how this works.
namespace Gordon360.Extensions
{
    public static class MiddlewareExtensions
    { 
        public static IApplicationBuilder UseFactoryActivatedMiddleware(
            this IApplicationBuilder app)
            => app.UseMiddleware<RequestResponseLoggerMiddleware>();
    }
    
}
