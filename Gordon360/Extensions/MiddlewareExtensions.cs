
using Gordon360.Utilities.Logger;
using Microsoft.AspNetCore.Builder;

namespace Gordon360.Extensions
{
    public static class MiddlewareExtensions
    { 
        public static IApplicationBuilder UseFactoryActivatedMiddleware(
            this IApplicationBuilder app)
            => app.UseMiddleware<RequestResponseLoggerMiddleware>();
    }
    
}
