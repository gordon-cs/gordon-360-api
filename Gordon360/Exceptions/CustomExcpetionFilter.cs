using System;
using System.Net.Http;
using System.Web.Http.Filters;
using Gordon360.Exceptions.CustomExceptions;

namespace Gordon360.Exceptions.ExceptionFilters
{
    public class CustomExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            
            if (actionExecutedContext.Exception is ResourceNotFoundException)
            {
                var exception = actionExecutedContext.Exception as ResourceNotFoundException;
                actionExecutedContext.Response = new HttpResponseMessage()
                {

                    StatusCode = System.Net.HttpStatusCode.NotFound,
                    Content = new StringContent(exception.ExceptionMessage),
                    ReasonPhrase = "LOL"
                };
            }

            else if (actionExecutedContext.Exception is ResourceCreationException)
            {
                var exception = actionExecutedContext.Exception as ResourceCreationException;
                actionExecutedContext.Response = new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.Conflict,
                    Content = new StringContent(exception.ExceptionMessage)
                };
            }

            else if( actionExecutedContext.Exception is BadInputException)
            {
                var exception = actionExecutedContext.Exception as BadInputException;
                actionExecutedContext.Response = new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Content = new StringContent(exception.ExceptionMessage)
                };
            }
        }
    }
}