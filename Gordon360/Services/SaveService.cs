using Gordon360.Database.CCT;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Models;
using Gordon360.Models.CCT;
using Gordon360.Services.ComplexQueries;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Services
{
    public class SaveService : ISaveService
    {
        private readonly CCTContext _context;

        public SaveService(CCTContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Fetch all upcoming ride items
        /// </summary>
        /// <returns> IEnumerable of ride items if found, null if not found</returns>
        public IEnumerable<UPCOMING_RIDESResult> GetUpcoming(string gordon_id)
        {
            var result = _context.Procedures.UPCOMING_RIDESAsync(int.Parse(gordon_id));

            if (result == null)
            {
                return null;
            }

            return (IEnumerable<UPCOMING_RIDESResult>)result;
        }

        /// <summary>
        /// Fetch the ride items a user is part of
        /// </summary>
        /// <param name="gordon_id">The ride id</param>
        /// <returns> ride items if found, null if not found</returns>
        public IEnumerable<UPCOMING_RIDES_BY_STUDENT_IDResult> GetUpcomingForUser(string gordon_id)
        {
            var result = _context.Procedures.UPCOMING_RIDES_BY_STUDENT_IDAsync(int.Parse(gordon_id));

            if (result == null)
            {
                return null;
            }

            return (IEnumerable<UPCOMING_RIDES_BY_STUDENT_IDResult>)result;
        }


        /// <summary>
        /// Adds a new ride record to storage.
        /// </summary>
        /// <param name="newRide">The Save_Rides object to be added</param>
        /// <param name="gordon_id">The gordon_id of the user creating the ride</param>
        /// <returns>The newly added custom event</returns>
        public async Task<Save_Rides> AddRide(Save_Rides newRide, string gordon_id)
        {

            // Assign event id
            var rideList = _context.Save_Rides.Where(x => x.rideID == newRide.rideID);
            int highestRideID = 1;
            foreach (var ride in rideList)
            {
                highestRideID += 1;
            }
            var newRideID = highestRideID.ToString();

            var result =  await _context.Procedures.CREATE_RIDEAsync(newRideID, newRide.destination, newRide.meetingPoint, newRide.startTime, newRide.endTime, newRide.capacity, newRide.notes, newRide.canceled);

            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The ride could not be created." };
            }

            await _context.Procedures.CREATE_BOOKINGAsync(gordon_id, newRideID, 1);

            _context.SaveChanges();

            return newRide;

        }

        /// <summary>
        /// Delete the ride whose id is specified by the parameter.
        /// </summary>
        /// <param name="rideID">The myschedule id</param>
        /// <param name="gordon_id">The gordon id</param>
        /// <returns>The myschedule that was just deleted</returns>
        public async Task<Save_Rides> DeleteRide(string rideID, string gordon_id)
        {
            //make get first or default then use generic repository delete on result
            var result = await _context.Save_Rides.FirstOrDefaultAsync(x => x.rideID == rideID);
            var booking = await _context.Save_Bookings.FirstOrDefaultAsync(x => x.ID == gordon_id && x.rideID == rideID && x.isDriver == 1);
            if (!(booking == null))
            {
                if (result == null)
                {
                    throw new ResourceNotFoundException() { ExceptionMessage = "The ride was not found." };
                }

                await _context.Procedures.DELETE_RIDEAsync(rideID); 
                await _context.Procedures.DELETE_BOOKINGSAsync(rideID); 
                _context.SaveChanges();
            }

            return result;
        }

        /// <summary>
        /// Cancel the ride whose id is specified by the parameter.
        /// </summary>
        /// <param name="rideID">The ride id</param>
        /// <param name="gordon_id">The gordon id</param>
        /// <returns>The ride that was just deleted</returns>
        public async Task<int> CancelRide(string rideID, string gordon_id)
        {
            var result = await _context.Procedures.CANCEL_RIDEAsync(int.Parse(gordon_id), rideID);
            _context.SaveChanges();

            return result;
        }

        /// <summary>
        /// Adds a new booking record to storage.
        /// </summary>
        /// <param name="newBooking">The Save_Bookings object to be added</param>
        /// <returns>The newly added custom event</returns>
        public async Task<Save_Bookings> AddBooking(Save_Bookings newBooking)
        {
            var ride = await _context.Save_Rides.FirstOrDefaultAsync(x => x.rideID == newBooking.rideID);
            var bookings = _context.Save_Bookings.Where(x => x.rideID == newBooking.rideID);
            int bookedCount = 0;
            foreach (var booking in bookings)
            {
                bookedCount += 1;
            }

            if (ride.capacity < bookedCount)
            {
                throw new ResourceCreationException() { ExceptionMessage = "Ride is full!" };
            }

            await _context.Procedures.CREATE_BOOKINGAsync(newBooking.ID, newBooking.rideID, newBooking.isDriver);

            await _context.SaveChangesAsync();

            return newBooking;
        }


        /// <summary>
        /// Delete the booking whose ids are specified by the parameter.
        /// </summary>
        /// <param name="rideID">The myschedule id</param>
        /// <param name="gordon_id">The gordon id</param>
        /// <returns>The myschedule that was just deleted</returns>
        public async Task<Save_Bookings> DeleteBooking(string rideID, string gordon_id)
        {
            var result = await _context.Save_Bookings.FirstOrDefaultAsync(x => x.ID == gordon_id && x.rideID == rideID);
            if (result != null)
            {
                // If driver booking is deleted, ride is deleted
                // TODO: notify users
                if (result.isDriver == 1)
                {
                    await _context.Procedures.DELETE_RIDEAsync(rideID);
                    await _context.SaveChangesAsync();
                }
                await _context.Procedures.DELETE_BOOKINGAsync(rideID, gordon_id);
                await _context.SaveChangesAsync();
            }

            return result;
        }

    }
}
