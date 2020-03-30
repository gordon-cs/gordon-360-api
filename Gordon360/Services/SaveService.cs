using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Models;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Services.ComplexQueries;
using System.Data.SqlClient;
using System.Data;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Static.Methods;
using System.Diagnostics;

namespace Gordon360.Services
{
    public class SaveService : ISaveService
    {
        private IUnitOfWork _unitOfWork;

        public SaveService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Fetch all upcoming ride items
        /// </summary>
        /// <returns> IEnumerable of ride items if found, null if not found</returns>
        public IEnumerable<RideViewModel> GetUpcoming()
        {
            var result = RawSqlQuery<RideViewModel>.query("UPCOMING_RIDES"); //run stored procedure

            if (result == null)
            {
                return null;
            }

            return result;
        }

        /// <summary>
        /// Fetch the ride items a user is part of
        /// </summary>
        /// <param name="gordon_id">The ride id</param>
        /// <returns> ride items if found, null if not found</returns>
        public IEnumerable<RideViewModel> GetUpcomingForUser(string gordon_id)
        {
            var idParam = new SqlParameter("@STUDENT_ID", gordon_id);
            var result = RawSqlQuery<RideViewModel>.query("UPCOMING_RIDES_BY_STUDENT_ID @STUDENT_ID", idParam); //run stored procedure

            if (result == null)
            {
                return null;
            }

            return result;
        }

        /// <summary>
        /// Fetch users in a ride specified by a ride_id
        /// </summary>
        /// <param name="rideID">The ride id</param>
        /// <returns> IEnumerable of user items if found, null if not found</returns>
        public IEnumerable<RideViewModel> GetUsersInRide(string rideID)
        {
            var idParam = new SqlParameter("@RIDE_ID", rideID);
            var result = RawSqlQuery<RideViewModel>.query("RIDERS_BY_RIDE_ID @RIDE_ID", idParam); //run stored procedure

            if (result == null)
            {
                return null;
            }

            return result;
        }

        /// <summary>
        /// Adds a new ride record to storage.
        /// </summary>
        /// <param name="rideID">The Save_Rides object to be added</param>
        /// <returns>The newly added custom event</returns>
        public Save_Rides GetRide(string rideID)
        {

            // Assign event id
            var ride = _unitOfWork.RideRepository.FirstOrDefault(x => x.rideID == rideID);
            if (ride == null)
            {
                throw new ResourceCreationException() { ExceptionMessage = "Ride event with this ID does not exist." };
            }
            return ride;
        }

            /// <summary>
            /// Adds a new ride record to storage.
            /// </summary>
            /// <param name="newRide">The Save_Rides object to be added</param>
            /// <param name="gordon_id">The gordon_id of the user creating the ride</param>
            /// <returns>The newly added custom event</returns>
            public Save_Rides AddRide(Save_Rides newRide, string gordon_id)
        {

            // Assign event id
            var rideList = _unitOfWork.RideRepository.GetAll(x => x.rideID == newRide.rideID);
            int highestRideID = 1;
            foreach (var ride in rideList)
            {
                highestRideID += 1;
            }
            var newRideID = highestRideID.ToString();

            var idParam = new SqlParameter("@ID", gordon_id);
            var rideIdParam = new SqlParameter("@RIDEID", newRide.rideID);
            var isDriverParam = new SqlParameter("@ISDRIVER", 1);

            var context = new CCTEntities1();

            var rideIdParam2 = new SqlParameter("@RIDEID", newRide.rideID);
            var destinationParam = new SqlParameter("@DESTINATION", newRide.destination);
            var meetingPointParam = new SqlParameter("@MEETINGPOINT", newRide.meetingPoint);
            var startTimeParam = new SqlParameter("@STARTTIME", newRide.startTime);
            var endTimeParam = new SqlParameter("@ENDTIME", newRide.endTime);
            var capacityParam = new SqlParameter("@CAPACITY", newRide.capacity);
            var notesParam = new SqlParameter("@NOTES", newRide.notes);
            var canceledParam = new SqlParameter("@CANCELED", newRide.canceled);
            context.Database.ExecuteSqlCommand("CREATE_RIDE @RIDEID, @DESTINATION, @MEETINGPOINT, @STARTTIME, @ENDTIME, @CAPACITY, @NOTES, @CANCELED", rideIdParam2, destinationParam, meetingPointParam, startTimeParam, endTimeParam, capacityParam, notesParam, canceledParam);

            context.Database.ExecuteSqlCommand("CREATE_BOOKING @ID, @RIDEID, @ISDRIVER", idParam, rideIdParam, isDriverParam); // run stored procedure.

            _unitOfWork.Save();

            return newRide;

        }

        /// <summary>
        /// Delete the ride whose id is specified by the parameter.
        /// </summary>
        /// <param name="rideID">The myschedule id</param>
        /// <param name="gordon_id">The gordon id</param>
        /// <returns>The myschedule that was just deleted</returns>
        public Save_Rides DeleteRide(string rideID, string gordon_id)
        {
            //make get first or default then use generic repository delete on result
            var result = _unitOfWork.RideRepository.FirstOrDefault(x => x.rideID == rideID);
            var booking = _unitOfWork.BookingRepository.FirstOrDefault(x => x.ID == gordon_id && x.rideID == rideID && x.isDriver == 1);
            if (!(booking == null))
            {
                if (result == null)
                {
                    throw new ResourceNotFoundException() { ExceptionMessage = "The ride was not found." };
                }
                var idParam = new SqlParameter("@RIDE_ID", rideID);
                var context = new CCTEntities1();

                context.Database.ExecuteSqlCommand("DELETE_RIDE @RIDE_ID", idParam); // run stored procedure.
                _unitOfWork.Save();
                var idParam2 = new SqlParameter("@RIDE_ID", rideID);
                context.Database.ExecuteSqlCommand("DELETE_BOOKINGS @RIDE_ID", idParam2); // run stored procedure.
                _unitOfWork.Save();
            }

            return result;
        }

        /// <summary>
        /// Adds a new booking record to storage.
        /// </summary>
        /// <param name="newBooking">The Save_Bookings object to be added</param>
        /// <returns>The newly added custom event</returns>
        public Save_Bookings AddBooking(Save_Bookings newBooking)
        {
            var ride = _unitOfWork.RideRepository.FirstOrDefault(x => x.rideID == newBooking.rideID);
            var bookings = _unitOfWork.BookingRepository.GetAll(x => x.rideID == newBooking.rideID);
            int bookedCount = 0;
            foreach (var booking in bookings)
            {
                bookedCount += 1;
            }

            if (ride.capacity < bookedCount)
            {
                throw new ResourceCreationException() { ExceptionMessage = "Ride is full!" };
            }

            var idParam = new SqlParameter("@ID", newBooking.ID);
            var rideIdParam = new SqlParameter("@RIDEID", newBooking.rideID);
            var isDriverParam = new SqlParameter("@ISDRIVER", newBooking.isDriver);
            var context = new CCTEntities1();
            context.Database.ExecuteSqlCommand("CREATE_BOOKING @ID, @RIDEID, @ISDRIVER", idParam, rideIdParam, isDriverParam);

            _unitOfWork.Save();

            return newBooking;
        }


        /// <summary>
        /// Delete the booking whose ids are specified by the parameter.
        /// </summary>
        /// <param name="rideID">The myschedule id</param>
        /// <param name="gordon_id">The gordon id</param>
        /// <returns>The myschedule that was just deleted</returns>
        public Save_Bookings DeleteBooking(string rideID, string gordon_id)
        {
            var result = _unitOfWork.BookingRepository.FirstOrDefault(x => x.ID == gordon_id && x.rideID == rideID);
            if (result != null)
            {
                var rideIDParam = new SqlParameter("@RIDE_ID", rideID);
                var gordonIDParam = new SqlParameter("@ID", gordon_id);
                var context = new CCTEntities1();
                // If driver booking is deleted, ride is deleted
                // TODO: notify users
                if (result.isDriver == 1)
                {
                    context.Database.ExecuteSqlCommand("DELETE_RIDE @RIDE_ID", rideIDParam); // run stored procedure.
                    _unitOfWork.Save();
                }
                context.Database.ExecuteSqlCommand("DELETE_BOOKING @RIDE_ID, @ID", rideIDParam, gordonIDParam); // run stored procedure.
                _unitOfWork.Save();
            }

            return result;
        }

        /// <summary>
        /// Fetch number of valid drives (1 or more passengers) by ID
        /// </summary>
        /// <param name="gordon_id">The gordon id</param>
        /// <returns> IEnumerable of user items if found, null if not found</returns>
        public int GetValidDrives(string gordon_id)
        {
            var idParam = new SqlParameter("@DRIVER_ID", gordon_id);
            var context = new CCTEntities1();
            var result = context.Database.ExecuteSqlCommand("VALID_DRIVES_BY_ID @DRIVER_ID", idParam); //run stored procedure
        
            return result;
        }

    }
}