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
         public IEnumerable<MyScheduleViewModel> GetAllForID(string gordon_id)
         {
             var query = _unitOfWork.MyScheduleRepository.GetById(gordon_id);
             if (query == null)
             {
                 throw new ResourceNotFoundException() { ExceptionMessage = "The user has no MySchedule objects." };
             }
      
             var idParam = new SqlParameter("@GORDON_ID", gordon_id);
             var result = RawSqlQuery<MyScheduleViewModel>.query("MYSCHEDULE_BY_ID @GORDON_ID", idParam); // TODO: write prepared statement
      
             if (result == null)
             {
                 return null;
             }
             // Getting rid of database-inherited whitespace DO WE NEED THIS? Does the ViewModel do it?
             foreach (var sch in result)
             {
                sch.Location = sch.Location.Trim() ?? ""; // For Null locations;
                sch.Description = sch.Description.Trim() ?? ""; // For Null descriptions
                sch.MonCode = sch.MonCode.Trim() ?? ""; // For Null days
                sch.TueCode = sch.TueCode.Trim() ?? ""; // For Null days
                sch.WedCode = sch.WedCode.Trim() ?? ""; // For Null days
                sch.ThuCode = sch.ThuCode.Trim() ?? ""; // For Null days
                sch.FriCode = sch.FriCode.Trim() ?? ""; // For Null days
                sch.SatCode = sch.SatCode.Trim() ?? ""; // For Null days
                sch.SunCode = sch.SunCode.Trim() ?? ""; // For Null days
            }
      
             return result;
         }
      
      
//         /// <summary>
//         /// Adds a new mySchedule record to storage. Since we can't establish foreign key constraints and relationships on the database side,
//         /// we might do it here by using something like the validateMembership() method.
//         /// </summary>
//         /// <param name="mySchedule">The membership to be added</param>
//         /// <returns>The newly added mySchedule object</returns>
//         public MYSCHEDULE Add(MYSCHEDULE mySchedule)
//         {
//             // validate returns a boolean value.
//             //validateMembership(membership);
//      
//             // The Add() method returns the added membership.
//             var payload = _unitOfWork.MyScheduleRepository.Add(mySchedule);
//      
//             // There is a unique constraint in the Database on columns (ID_NUM, BEGIN_DTE, END_DTE and DSCRIPT_TXT)
//             if (payload == null)
//             {
//                 throw new ResourceCreationException() { ExceptionMessage = "There was an error adding the myschedule event. Verify that a similar schedule doesn't already exist." };
//             }
//             _unitOfWork.Save();
//      
//             return payload;
//      
//         }
//      
//         /// <summary>
//         /// Delete the myschedule whose id is specified by the parameter. Should we have a myschedule id?
//         /// </summary>
//         /// <param name="id">The myschedule id</param>
//         /// <returns>The myschedule that was just deleted</returns>
//         public MYSCHEDULE Delete(string schedule_id, string gordon_id)
//         {
//             var result = _unitOfWork.MyScheduleRepository.GetById(id);
//             if (result == null)
//             {
//                 throw new ResourceNotFoundException() { ExceptionMessage = "The MySchedule was not found." };
//             }
//             result = _unitOfWork.MyScheduleRepository.Delete(result);
//      
//             _unitOfWork.Save();
//      
//             return result;
//         }
//
//         /// <summary>
//         /// Delete all myschedule items for a student whose id is specified by the parameter. Should we have a schedule id?
//         /// </summary>
//         /// <param name="id">The student's id</param>
//         /// <returns>The schedule that was just deleted</returns>
//         public MYSCHEDULE Update(string user_id, MYSCHEDULE sched)
//         {
//            var event_id = sched.EVENT_ID;
//            
//            var original = _unitOfWork.ActivityInfoRepository.GetById(event_id, user_id);
//
//            if (original == null)
//            {
//                throw new ResourceNotFoundException() { ExceptionMessage = "The Schedule Info was not found." };
//            }
//
//            // One can only update certain fields within a membrship
//            original.ACT_BLURB = sched.ACT_BLURB;
//            original.ACT_URL = sched.ACT_URL;
//            original.ACT_JOIN_INFO = sched.ACT_JOIN_INFO;
//
//            _unitOfWork.Save();
//
//            return original;
//
//            }
//
//        }
    }
}