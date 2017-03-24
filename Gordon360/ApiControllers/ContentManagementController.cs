using System;
using System.Web.Http;
using Gordon360.Services;
using Gordon360.Repositories;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Static.Methods;

namespace Gordon360.Controllers.Api
{
    [RoutePrefix("api/cms")]
    [Authorize]
    [CustomExceptionFilter]
    public class ContentManagementController : ApiController
    {
    }
}
