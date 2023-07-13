using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
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

        /// <summary>
        /// Invoke Async is called on all HTTP requests and are intercepted by httpContext pipelining
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="next"></param>
        /// <returns></returns>
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

            /*request*/
            log.RequestMethod = request.Method;
            log.RequestPath = request.Path;
            log.RequestQuery = request.QueryString.ToString();
            log.UserAgent = request.Headers.UserAgent;
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

        /// <summary>
        /// Picks User-Agent out of given headers, defaults to empty string
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        private string GetUserAgent(IHeaderDictionary headers)
        {
            if (headers.UserAgent is StringValues UA) 
                return UA;
            return "";
        }

        /// <summary>
        /// Returns length of response content, defaults to 0
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        private int GetResponseContentLength(IHeaderDictionary headers)
        {
            if (headers.ContentLength is long contentLenth)
                return (int)contentLenth;
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