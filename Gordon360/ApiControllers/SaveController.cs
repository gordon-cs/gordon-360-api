using Gordon360.Models.CCT;
using Gordon360.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Gordon360.ApiControllers
{
    public class SaveController : GordonControllerBase
    {
        private readonly ISaveService _saveService;

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
        public ActionResult<IEnumerable<UPCOMING_RIDESResult>> GetUpcomingRides()
        {
            var authenticatedUserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var result = _saveService.GetUpcoming(authenticatedUserIdString);

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
        public ActionResult<IEnumerable<UPCOMING_RIDES_BY_STUDENT_IDResult>> GetUpcomingRidesForUser()
        {
            var authenticatedUserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var result = _saveService.GetUpcomingForUser(authenticatedUserIdString);

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
        public ActionResult<Save_Rides> PostRide([FromBody] Save_Rides newRide)
        {
            var authenticatedUserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var result = _saveService.AddRide(newRide, authenticatedUserIdString);

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
        public async Task<ActionResult<int>> CancelRide(string rideID)
        {
            var authenticatedUserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var result = await _saveService.CancelRide(rideID, authenticatedUserIdString);

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
        public ActionResult<Save_Rides> DeleteRide(string rideID)
        {
            var authenticatedUserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var result = _saveService.DeleteRide(rideID, authenticatedUserIdString);

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
        public ActionResult<Save_Bookings> PostBooking([FromBody] Save_Bookings newBooking)
        {
            var authenticatedUserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            newBooking.ID = authenticatedUserIdString;

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
        public ActionResult<Save_Bookings> DeleteBooking(string rideID)
        {
            var authenticatedUserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var result = _saveService.DeleteBooking(rideID, authenticatedUserIdString);

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
