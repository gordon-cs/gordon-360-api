using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Gordon360.Utilities.Logger
{
    public class RequestResponseLoggerMiddleware : IMiddleware
    {
        private readonly RequestResponseLoggerOptionModel _options;
        private readonly CCTContext _context;
        public RequestResponseLoggerMiddleware
        (IOptions<RequestResponseLoggerOptionModel> options, CCTContext context)
        {
            _options = options.Value;
            _context = context;
        }

        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            RequestResponseLog log = new RequestResponseLog();
            // Middleware is enabled only when the 
            // EnableRequestResponseLogging config value is set.
            if (_options == null || !_options.IsEnabled)
            {
                await next(httpContext);
                return;
            }
            log.RequestDateTime = DateTime.UtcNow;
            HttpRequest request = httpContext.Request;

            /*log*/
            log.LogID = Guid.NewGuid().ToString();
            var ip = request.HttpContext.Connection.RemoteIpAddress;
            log.ClientIP = ip == null ? "" : ip.ToString();
            request.HttpContext.Connection.

            /*request*/
            log.RequestMethod = request.Method;
            log.RequestPath = request.Path;
            log.RequestQuery = request.QueryString.ToString();
            log.UserAgent = GetUserAgent(request.Headers);
            log.RequestBody = await ReadBodyFromRequest(request);
            log.RequestHost = request.Host.ToString();

            // Temporarily replace the HttpResponseStream, 
            // which is a write-only stream, with a MemoryStream to capture 
            // its value in-flight.
            HttpResponse response = httpContext.Response;
            var originalResponseBody = response.Body;
            using var newResponseBody = new MemoryStream();
            response.Body = newResponseBody;

            // Call the next middleware in the pipeline
            try
            {
                await next(httpContext);
            }
            catch
            {

            }

            newResponseBody.Seek(0, SeekOrigin.Begin);
            var responseBodyText =
                await new StreamReader(response.Body).ReadToEndAsync();

            newResponseBody.Seek(0, SeekOrigin.Begin);
            await newResponseBody.CopyToAsync(originalResponseBody);

            log.ResponseStatus = response.StatusCode;
            log.ResponseContentLength = GetResponseContentLength(response.Headers);
            _context.RequestResponseLog.Add(log);
            _context.SaveChanges();
        }

        private string GetUserAgent(IHeaderDictionary headers)
        {
            foreach (var header in headers) 
                if(header.Key == "User-Agent")
                    return header.Value;
            return "";
        }

        private int GetResponseContentLength(IHeaderDictionary headers)
        {
            foreach (var header in headers)
                if (header.Key == "Content-Length")
                    return Int32.Parse(header.Value);
            return 0;
        }

        private async Task<string> ReadBodyFromRequest(HttpRequest request)
        {
            // Ensure the request's body can be read multiple times 
            // (for the next middlewares in the pipeline).
            request.EnableBuffering();
            using var streamReader = new StreamReader(request.Body, leaveOpen: true);
            var requestBody = await streamReader.ReadToEndAsync();
            // Reset the request's body stream position for 
            // next middleware in the pipeline.
            request.Body.Position = 0;
            return requestBody;
        }
    }
}