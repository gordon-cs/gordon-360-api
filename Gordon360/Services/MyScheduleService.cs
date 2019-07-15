﻿using System;
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
    /// <summary>
    /// Service Class that facilitates data transactions between the MySchedulesController and the MySchedule part of the database model.
    /// </summary>
    public class MyScheduleService : IMyScheduleService
    {
        private IUnitOfWork _unitOfWork;

        public MyScheduleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Fetch the myschedule item whose id is specified by the parameter
        /// </summary>
        /// <param name="gordon_id">The myschedule id</param>
        /// <returns>MyScheduleViewModel if found, null if not found</returns>
         public IEnumerable<MYSCHEDULE> GetAllForID(string gordon_id)
         {

            // Account Verification
            var account = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == gordon_id);

            if (account == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var idParam = new SqlParameter("@GORDON_ID", gordon_id);

            var context = new CCTEntities1();
            var result = _unitOfWork.MyScheduleRepository.GetAll(x => x.GORDON_ID == gordon_id);
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
        /// <returns>The newly added mySchedule object</returns>
        public MYSCHEDULE Add(MYSCHEDULE mySchedule)
        {

            // Account verification
            var account = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == mySchedule.GORDON_ID);

            if (account == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }


            // Assign event id
            var myScheduleList = _unitOfWork.MyScheduleRepository.GetAll(x => x.GORDON_ID == mySchedule.GORDON_ID);
            int largestEventId = 0;
            int i = 0;
            if (myScheduleList == null)
            {
                mySchedule.EVENT_ID = "1000";
            }
            else
            {
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
            }


            // The Add() method returns the added schedule
            var payload = _unitOfWork.MyScheduleRepository.Add(mySchedule);

            // There is a unique constraint in the Database on columns
            if (payload == null)
            {
                throw new ResourceCreationException() { ExceptionMessage = "There was an error adding the myschedule event. Verify that a similar schedule doesn't already exist." };
            }
            _unitOfWork.Save();

            return payload;

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
            var account = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == gordon_id);
            if (account == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var result = _unitOfWork.MyScheduleRepository.FirstOrDefault(x => x.GORDON_ID == gordon_id && x.EVENT_ID == event_id);
            if (result == null)
             {
                 throw new ResourceNotFoundException() { ExceptionMessage = "The MySchedule was not found." };
             }

            var idParam = new SqlParameter("@GORDONID", gordon_id);
            var eventIdParam = new SqlParameter("@EVENTID", event_id);
            var context = new CCTEntities1();
            context.Database.ExecuteSqlCommand("DELETE_MYSCHEDULE @GORDONID, @EVENTID", idParam, eventIdParam); // run stored procedure.

            _unitOfWork.Save();
      
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
            var account = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == gordon_id);
            if (account == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }
            
            var original = _unitOfWork.MyScheduleRepository.FirstOrDefault(x => x.GORDON_ID == gordon_id && x.EVENT_ID == event_id);

            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The MySchedule was not found." };
            }

            var eventIdParam = new SqlParameter("@EVENTID", sched.EVENT_ID);
            var idParam = new SqlParameter("@GORDONID", sched.GORDON_ID);
            var locationParam = new SqlParameter("@LOCATION", sched.LOCATION);
            var descriptionParam = new SqlParameter("@DESCRIPTION", sched.DESCRIPTION);
            var monCdeParam = new SqlParameter("@MON_CDE", sched.MON_CDE);
            var tueCdeParam = new SqlParameter("@TUE_CDE", sched.TUE_CDE);
            var wedCdeParam = new SqlParameter("@WED_CDE", sched.WED_CDE);
            var thuCdeParam = new SqlParameter("@THU_CDE", sched.THU_CDE);
            var friCdeParam = new SqlParameter("@FRI_CDE", sched.FRI_CDE);
            var satCdeParam = new SqlParameter("@SAT_CDE", sched.SAT_CDE);
            var sunCdeParam = new SqlParameter("@SUN_CDE", sched.SUN_CDE);
            var allDayParam = new SqlParameter("@IS_ALLDAY", sched.IS_ALLDAY);
            var beginTimeParam = new SqlParameter("@BEGINTIME", sched.BEGIN_TIME);
            var endTimeParam = new SqlParameter("@ENDTIME", sched.END_TIME);
            var context = new CCTEntities1();
            context.Database.ExecuteSqlCommand("UPDATE_MYSCHEDULE " +
                "@EVENTID, @GORDONID @LOCATION @DESCRIPTION @MON_CDE @TUE_CDE @WED_CDE" +
                "@THU_CDE @FRI_CDE @SAT_CDE @SUN_CDE @IS_ALLDAY @BEGINTIME @ENDTIME"
                , eventIdParam, idParam, locationParam, descriptionParam, monCdeParam, tueCdeParam,
                wedCdeParam, thuCdeParam, friCdeParam,satCdeParam, sunCdeParam, allDayParam,
                beginTimeParam, endTimeParam); // run stored procedure.

            _unitOfWork.Save();

            return original;

         }

    }
}
