

namespace Gordon360.Controllers.Api
{
    [RoutePrefix("api/checkIn")]
    [Authorize]
    [CustomExceptionFilter]
    public class AcademicCheckInController : ApiController 
    {
        private IAcadmicCheckInService _checkInService;
                
        public AcademicCheckInController() 
        {
            IUnitOfWork _unitOfWork = new UnitOfWork();
            _checkInService = new AcademicCheckInService(_unitOfWork);
        }
        
        /// <summary>Gets a student's holds by id from the database</summary>
        /// <param name="studentID">The id of the student to retrieve the holds of</param>
        /// <returns>The student's current holds (if any)</returns>
        [HttpGet]
        [Route("holds")]
        // TO DO: Add StateYourBusiness??
        // Private route to authenticated users
        public IHttpActionResult GetHolds()
        {
            // Get authenticated username/id
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var id = _accountService.GetAccountByUsername(username).GordonID;

            var result = _checkInService.GetHolds(id);

            if (result == null){
                return NotFound();
            }
            return Ok(result);
        }

        public IHTTPActionResult getDemographics()
        {
            // Get authenticated username/id
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var id = _accountService.GetAccountByUsername(username).GordonID;

            var result = _checkInService.getDemographics(id);

            if (result == null){
                return NotFound();
            }
            return Ok(result);
        }

    }
}