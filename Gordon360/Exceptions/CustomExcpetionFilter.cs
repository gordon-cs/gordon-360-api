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
                actionExecutedContext.Response = new System.Net.Http.HttpResponseMessage()
                {

                    StatusCode = System.Net.HttpStatusCode.NotFound,
                    Content = new StringContent(exception.ExceptionMessage)
                };
            }
        }
    }
}