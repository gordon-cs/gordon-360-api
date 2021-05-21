using System.Net.Http;
using System.Web.Http.Filters;
using Gordon360.Exceptions.CustomExceptions;

/// <summary>
/// These exception filters catch unhandled exceptions and convert them
/// into HTTP Responses
/// </summary>
namespace Gordon360.Exceptions.ExceptionFilters
{
    public class CustomExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            // RESOURCE NOT FOUND
            if (actionExecutedContext.Exception is ResourceNotFoundException)
            {
                var exception = actionExecutedContext.Exception as ResourceNotFoundException;
                actionExecutedContext.Response = actionExecutedContext.Request.CreateErrorResponse(System.Net.HttpStatusCode.NotFound, exception.ExceptionMessage);
            }

            // RESOURCE CREATION CONFLICT
            else if (actionExecutedContext.Exception is ResourceCreationException)
            {
                var exception = actionExecutedContext.Exception as ResourceCreationException;
                actionExecutedContext.Response = actionExecutedContext.Request.CreateErrorResponse(System.Net.HttpStatusCode.Conflict, exception.ExceptionMessage);
            }

            // BAD INPUT
            else if( actionExecutedContext.Exception is BadInputException)
            {
                var exception = actionExecutedContext.Exception as BadInputException;
                actionExecutedContext.Response = actionExecutedContext.Request.CreateErrorResponse(System.Net.HttpStatusCode.BadRequest, exception.ExceptionMessage);
            }

            // UNAUTHORIZED ACCESS EXCEPTION
            else if (actionExecutedContext.Exception is UnauthorizedAccessException)
            {
                var exception = actionExecutedContext.Exception as UnauthorizedAccessException;
                actionExecutedContext.Response = actionExecutedContext.Request.CreateErrorResponse(System.Net.HttpStatusCode.Unauthorized, exception.ExceptionMessage);
            }
        }
    }
}