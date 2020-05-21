using System.Web.Http;
using Gordon360.Models;
using Gordon360.Services;
using Gordon360.Repositories;
using Gordon360.Models.ViewModels;
using Gordon360.AuthorizationFilters;
using Gordon360.Static.Names;
using System;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Exceptions.CustomExceptions;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;

namespace Gordon360.ApiControllers
{
    [RoutePrefix("api/save")]
    [Authorize]
    [CustomExceptionFilter]
    public class SaveController : ApiController
    {

        private ISaveService _saveService;
        private IAccountService _accountService;
        private IRoleCheckingService _roleCheckingService;

        public SaveController()
        {
            IUnitOfWork _unitOfWork = new UnitOfWork();
            _saveService = new SaveService(_unitOfWork);
            _accountService = new AccountService(_unitOfWork);
            _roleCheckingService = new RoleCheckingService(_unitOfWork);
        }
        public SaveController(ISaveService saveService)
        {
            _saveService = saveService;
        }

        /// <summary>
        ///  Gets all upcoming ride objects
        /// </summary>
        /// <returns>A IEnumerable of rides objects</returns>
        [HttpGet]
        [Route("rides")]
        public IHttpActionResult GetUpcomingRides()
        {

            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var id = _accountService.GetAccountByUsername(username).GordonID;

            var result = _saveService.GetUpcoming(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);

        }

        /// <summary>
        ///  Gets all upcoming ride objects for a user
        /// </summary>
        /// <returns>A IEnumerable of rides objects</returns>
        [HttpGet]
        [Route("myrides")]
        public IHttpActionResult GetUpcomingRidesForUser()
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var id = _accountService.GetAccountByUsername(username).GordonID;

            var result = _saveService.GetUpcomingForUser(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);

        }

        /// <summary>
        ///  Create new ride object for a user
        /// </summary>
        /// <returns>Successfully posted ride object</returns>
        [HttpPost]
        [Route("rides/add")]
        public IHttpActionResult PostRide([FromBody] Save_Rides newRide)
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var id = _accountService.GetAccountByUsername(username).GordonID;

            var result = _saveService.AddRide(newRide, id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);

        }

        /// <summary>Cancel an existing ride item</summary>
        /// <param name="rideID">The identifier for the ride to be cancel</param>
        /// <remarks>Calls the server to make a call and remove the given ride from the database</remarks>
        [HttpPut]
        [Route("rides/cancel/{rideID}")]
        public IHttpActionResult CancelRide(string rideID)
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var id = _accountService.GetAccountByUsername(username).GordonID;

            var result = _saveService.CancelRide(rideID, id);

            if (result != 0)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>Delete an existing ride item</summary>
        /// <param name="rideID">The identifier for the ride to be deleted</param>
        /// <remarks>Calls the server to make a call and remove the given ride from the database</remarks>
        [HttpDelete]
        [Route("rides/del/{rideID}")]
        public IHttpActionResult DeleteRide(string rideID)
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var id = _accountService.GetAccountByUsername(username).GordonID;

            var result = _saveService.DeleteRide(rideID, id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        ///  Create new booking object for a user
        /// </summary>
        /// <returns>Successfully posted booking object</returns>
        [HttpPost]
        [Route("books/add")]
        public IHttpActionResult PostBooking([FromBody] Save_Bookings newBooking)
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var id = _accountService.GetAccountByUsername(username).GordonID;
            newBooking.ID = id;

            var result = _saveService.AddBooking(newBooking);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);

        }

        /// <summary>Delete an existing booking item</summary>
        /// <param name="rideID">The identifier for the booking to be deleted</param>
        /// <remarks>Calls the server to make a call and remove the given booking from the database</remarks>
        [HttpDelete]
        [Route("books/del/{rideID}")]
        public IHttpActionResult DeleteBooking(string rideID)
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var id = _accountService.GetAccountByUsername(username).GordonID;

            var result = _saveService.DeleteBooking(rideID, id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        ///// <summary>
        /////  Gets all users in a ride
        ///// </summary>
        ///// <returns>A IEnumerable of riders objects</returns>
        //[HttpGet]
        //[Route("book/{ride_id}")]
        //public IHttpActionResult GetUsersInRide(string ride_id)
        //{
        //
        //    var result = _saveService.GetUsersInRide(ride_id);
        //    if (result == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(result);
        //
        //}

        ///// <summary>
        /////  Gets all users in a ride
        ///// </summary>
        ///// <returns>A IEnumerable of users objects</returns>
        //[HttpGet]
        //[Route("rides/{rideID}")]
        //public IHttpActionResult GetUsersInRide(int rideID)
        //{

        //    var result = _saveService.GetUsersInRide(rideID);
        //    if (result == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(result);

        //}

        ///// <summary>
        /////  Gets all qualifying drives given by a user (at least one passenger)
        ///// </summary>
        ///// <returns>A number</returns>
        //[HttpGet]
        //[Route("history/{username}")]
        //public IHttpActionResult GetValidDrives(string username)
        //{
        //    var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
        //    var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

        //    var id = _accountService.GetAccountByUsername(username).GordonID;

        //    var result = _saveService.GetValidDrives(id);
        //    if (result == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(result);

        //}

        ///// <summary>Create a new myschedule to be added to database</summary>
        ///// <param name="mySchedule">The myschedule item containing all required and relevant information</param>
        ///// <returns>Created schedule</returns>
        ///// <remarks>Posts a new myschedule to the server to be added into the database</remarks>
        //[HttpPost]
        //[Route("rides/create")]
        //public IHttpActionResult Post([FromBody] RIDE newRide)
        //{

        //    // Verify Input
        //    if (!ModelState.IsValid || newRide == null)
        //    {
        //        string errors = "";
        //        foreach (var modelstate in ModelState.Values)
        //        {
        //            foreach (var error in modelstate.Errors)
        //            {
        //                errors += "|" + error.ErrorMessage + "|" + error.Exception;
        //            }

        //        }
        //        throw new BadInputException() { ExceptionMessage = errors };
        //    }

        //    // Check if maximum
        //    var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
        //    var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

        //    var id = _accountService.GetAccountByUsername(username).GordonID;

        //    object existingEvents = _mySaveService.GetUpcomingById(id);

        //    JArray jEvents = JArray.FromObject(existingEvents);

        //    if (jEvents.Count > MAX)
        //    {
        //        return Unauthorized();
        //    }



        //    var result = _mySaveService.Add(myRide);

        //    if (result == null)
        //    {
        //        return NotFound();
        //    }


        //    return Created("myride", myRide);
        //}
    }
}