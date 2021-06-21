using System.Net.Http;
using Gordon360.Exceptions.CustomExceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

/// <summary>
/// These exception filters catch unhandled exceptions and convert them
/// into HTTP Responses
/// </summary>
namespace Gordon360.Exceptions.ExceptionFilters
{
    public class CustomExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext actionExecutedContext)
        {
            // RESOURCE NOT FOUND
            if (actionExecutedContext.Exception is ResourceNotFoundException)
            {
                var exception = actionExecutedContext.Exception as ResourceNotFoundException;
                
                /*********** new opt. 1 *********/
                actionExecutedContext.ExceptionHandled = true;
                // log error in dbS
                actionExecutedContext.Result = new ViewResult()
                {
                    ViewName = exception.ExceptionMessage
                };
                
                /********** new opt. 2 ************/
                //actionExecutedContext.ExceptionHandled = true;
                // log error in db
                //actionExecutedContext.Result = RedirectToAction(exception.ExceptionMessage, "InternalError");
                
                /*********** original *********/
                // actionExecutedContext.Response = actionExecutedContext.Request.CreateErrorResponse(System.Net.HttpStatusCode.NotFound, exception.ExceptionMessage);
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