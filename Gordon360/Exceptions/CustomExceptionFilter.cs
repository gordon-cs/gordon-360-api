using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;

// These exception filters catch unhandled exceptions and convert them into HTTP Responses
namespace Gordon360.Exceptions
{
    // Use on Controllers (classes) and Actions (methods)
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var statusCode = HttpStatusCode.InternalServerError;

            switch (context.Exception)
            {
                case ResourceNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    break;

                case ResourceCreationException:
                    statusCode = HttpStatusCode.Conflict;
                    break;

                case BadInputException:
                    statusCode = HttpStatusCode.BadRequest;
                    break;

                case UnprocessibleEntity:
                    statusCode = HttpStatusCode.UnprocessableEntity;
                    break;

                // Not a custom exception
                case UnauthorizedAccessException:
                    statusCode = HttpStatusCode.Unauthorized;
                    break;
            }

            // Create Http Response
            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.StatusCode = (int)statusCode;
            context.Result = new JsonResult(new
            {
                error = new[] { context.Exception.Message },
                stackTrace = context.Exception.StackTrace
            });
        }
    }
}
