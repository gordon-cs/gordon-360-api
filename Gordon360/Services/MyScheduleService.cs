using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Models.CCT;
using Gordon360.Database.CCT;

namespace Gordon360.Services
{
    /// <summary>
    /// Service Class that facilitates data transactions between the MySchedulesController and the MySchedule part of the database model.
    /// </summary>
    public class MyScheduleService : IMyScheduleService
    {
        private CCTContext _context;

        public MyScheduleService(CCTContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Fetch the myschedule item whose id is specified by the parameter
        /// </summary>
        /// <param name="event_id">The myschedule id</param>
        /// <param name="gordon_id">The gordon id</param>
        /// <returns>Myschedule if found, null if not found</returns>
        public MYSCHEDULE GetForID(string event_id, string gordon_id)
        {
            // Account Verification
            var account = _context.ACCOUNT.FirstOrDefault(x => x.gordon_id == gordon_id);
            if (account == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var result = _context.MYSCHEDULE.FirstOrDefault(x => x.GORDON_ID == gordon_id && x.EVENT_ID == event_id);
            if (result == null)
            {
                return null;
            }

            return result;
        }


        /// <summary>
        /// Fetch all myschedule items whose id is specified by the parameter
        /// </summary>
        /// <param name="gordon_id">The gordon id</param>
        /// <returns>Array of Myschedule if found, null if not found</returns>
        public IEnumerable<MYSCHEDULE> GetAllForID(string gordon_id)
        {

            // Account Verification
            var account = _context.ACCOUNT.FirstOrDefault(x => x.gordon_id == gordon_id);

            if (account == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var result = _context.MYSCHEDULE.Where(x => x.GORDON_ID == gordon_id);
            if (result == null)
            {
                return null;
            }

            return result;
        }




        /// <summary>
        /// Adds a new mySchedule record to storage.
        /// </summary>
        /// <param name="mySchedule">The membership to be added</param>
        /// <returns>The newly added custom event</returns>
        public MYSCHEDULE Add(MYSCHEDULE mySchedule)
        {

            // Account verification
            var account = _context.ACCOUNT.FirstOrDefault(x => x.gordon_id == mySchedule.GORDON_ID);

            if (account == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            // Assign event id
            var myScheduleList = _context.MYSCHEDULE.Where(x => x.GORDON_ID == mySchedule.GORDON_ID);
            int largestEventId = 1000;
            int i = 0;
                foreach (var schedule in myScheduleList)
                {

                    if (!Int32.TryParse(schedule.EVENT_ID, out i))
                    {
                        i = -1;
                    }
                    if (largestEventId < i)
                    {
                        largestEventId = i;
                    }
                }
                largestEventId++;
                mySchedule.EVENT_ID = largestEventId.ToString();


            // The Add() method returns the added schedule
            var payload = _context.MYSCHEDULE.Add(mySchedule);

            // There is a unique constraint in the Database on columns
            if (payload == null)
            {
                throw new ResourceCreationException() { ExceptionMessage = "There was an error adding the myschedule event. Verify that a similar schedule doesn't already exist." };
            }
            _context.SaveChanges();

            return mySchedule;

        }

        /// <summary>
        /// Delete the myschedule whose id is specified by the parameter.
        /// </summary>
        /// <param name="event_id">The myschedule id</param>
        /// <param name="gordon_id">The gordon id</param>
        /// <returns>The myschedule that was just deleted</returns>
        public MYSCHEDULE Delete(string event_id, string gordon_id)
        {
            // Account Verification
            var account = _context.ACCOUNT.FirstOrDefault(x => x.gordon_id == gordon_id);
            if (account == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var result = _context.MYSCHEDULE.FirstOrDefault(x => x.GORDON_ID == gordon_id && x.EVENT_ID == event_id);
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The MySchedule was not found." };
            }

            _context.MYSCHEDULE.Remove(new MYSCHEDULE { EVENT_ID = event_id, GORDON_ID = gordon_id });
            _context.SaveChanges();

            return result;
        }

        /// <summary>
        /// Update the myschedule item.
        /// </summary>
        /// <param name="sched">The schedule information</param>
        /// <returns>The original schedule</returns>
        public MYSCHEDULE Update(MYSCHEDULE sched)
        {

            var gordon_id = sched.GORDON_ID;
            var event_id = sched.EVENT_ID;

            // Account Verification
            var account = _context.ACCOUNT.FirstOrDefault(x => x.gordon_id == gordon_id);
            if (account == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var original = _context.MYSCHEDULE.FirstOrDefault(x => x.GORDON_ID == gordon_id && x.EVENT_ID == event_id);
            

            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The MySchedule was not found." };
            }

            // Update value only if new value was given
            original.LOCATION = sched.LOCATION ?? original.LOCATION;
            original.DESCRIPTION = sched.DESCRIPTION ?? original.DESCRIPTION;
            original.MON_CDE = sched.MON_CDE ?? original.MON_CDE;
            original.TUE_CDE = sched.TUE_CDE ?? original.TUE_CDE;
            original.WED_CDE = sched.WED_CDE ?? original.WED_CDE;
            original.THU_CDE = sched.THU_CDE ?? original.THU_CDE;
            original.FRI_CDE = sched.FRI_CDE ?? original.FRI_CDE;
            original.SAT_CDE = sched.SAT_CDE ?? original.SAT_CDE;
            original.SUN_CDE = sched.SUN_CDE ?? original.SUN_CDE;
            original.IS_ALLDAY = sched.IS_ALLDAY ?? original.IS_ALLDAY;
            original.BEGIN_TIME = sched.BEGIN_TIME ?? original.BEGIN_TIME;
            original.END_TIME = sched.END_TIME ?? original.END_TIME;

            _context.SaveChanges();

            return original;

        }

    }
}