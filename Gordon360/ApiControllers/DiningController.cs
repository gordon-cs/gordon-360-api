using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Repositories;
using Gordon360.Services;
using System;
using Gordon360.Static.Methods;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Gordon360.ApiControllers
{
    [Route("api/dining")]
    [CustomExceptionFilter]
    [Authorize]
    public class DiningController : ControllerBase
    {
        public readonly IDiningService _diningService;
        private const string FACSTAFF_MEALPLAN_ID = "7295";
        public DiningController()
        {
            IUnitOfWork _unitOfWork = new UnitOfWork();
            _diningService = new DiningService(_unitOfWork);
        }

        /// <summary>
        ///  Gets information about student's dining plan and balance
        /// </summary>
        /// <returns>A DiningInfo object</returns>
        [HttpGet]
        [Route("")]
        public ActionResult<string> Get()
        {
            if (!ModelState.IsValid)
            {
                string errors = "";
                foreach (var modelstate in ModelState.Values)
                {
                    foreach (var error in modelstate.Errors)
                    {
                        errors += "|" + error.ErrorMessage + "|" + error.Exception;
                    }
                }
                throw new BadInputException() { ExceptionMessage = errors };
            }

            var sessionCode = Helpers.GetCurrentSession().SessionCode;
            var authenticatedUserId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var diningInfo = _diningService.GetDiningPlanInfo(authenticatedUserId, sessionCode);

            if (diningInfo == null)
            {
                return NotFound();
            }
            if (diningInfo.ChoiceDescription == "None")
            {
                var diningBalance = _diningService.GetBalance(authenticatedUserId, FACSTAFF_MEALPLAN_ID);
                if (diningBalance == null)
                {
                    return NotFound();
                }
                return Ok(diningBalance);
            }
            else
            {
                return Ok(diningInfo);
            }

        }
    }
}
