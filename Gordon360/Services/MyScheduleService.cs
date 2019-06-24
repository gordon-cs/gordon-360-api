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
        /// <param name="id">The myschedule id</param>
        /// <returns>MyScheduleViewModel if found, null if not found</returns>
        public IEnumerable<MyScheduleViewModel> Get(int id)
        {
            var query = _unitOfWork.MyScheduleRepository.GetById(id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The MySchedule was not found." };
            }

            var idParam = new SqlParameter("@MYSCHEDULE_ID", id);
            var result = RawSqlQuery<MyScheduleViewModel>.query("MYSCHEDULE_PER_MYSCHEDULE_ID @MYSCHEDULE_ID", idParam); // TODO: write prepared statement

            if (result == null)
            {
                return null;
            }
            // Getting rid of database-inherited whitespace
            foreach (var myScheduleItem in result)
            {
                myScheduleItem.Title = myScheduleItem.Title.Trim();
                myScheduleItem.ItemDescription = myScheduleItem.ItemDescription.Trim();
            }

            return result;
        }


        /// <summary>
        /// Adds a new mySchedule record to storage. Since we can't establish foreign key constraints and relationships on the database side,
        /// we might do it here by using something like the validateMembership() method.
        /// </summary>
        /// <param name="myschedule">The membership to be added</param>
        /// <returns>The newly added mySchedule object</returns>
        public MYSCHEDULE Add(MYSCHEDULE mySchedule)
        {
            // validate returns a boolean value.
            //validateMembership(membership);

            // The Add() method returns the added membership.
            var payload = _unitOfWork.MyScheduleRepository.Add(mySchedule);

            // There is a unique constraint in the Database on columns (ID_NUM, BEGIN_DTE, END_DTE and DSCRIPT_TXT)
            if (payload == null)
            {
                throw new ResourceCreationException() { ExceptionMessage = "There was an error adding the myschedule event. Verify that a similar schedule doesn't already exist." };
            }
            _unitOfWork.Save();

            return payload;

        }

        /// <summary>
        /// Delete the myschedule whose id is specified by the parameter. Should we have a myschedule id?
        /// </summary>
        /// <param name="id">The myschedule id</param>
        /// <returns>The myschedule that was just deleted</returns>
        public MYSCHEDULE Delete(int id)
        {
            var result = _unitOfWork.MyScheduleRepository.GetById(id);
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The MySchedule was not found." };
            }
            result = _unitOfWork.MyScheduleRepository.Delete(result);

            _unitOfWork.Save();

            return result;
        }

        // /// <summary>
        // /// Delete all myschedule items for a student whose id is specified by the parameter. Should we have a schedule id?
        // /// </summary>
        // /// <param name="id">The student's id</param>
        // /// <returns>The schedule that was just deleted</returns>
        // public IEnumerable<ScheduleViewModel> DeleteAllForID(int id)
        // {
        //     // Confirm that student exists
        //     //var studentExists = _unitOfWork.AccountRepository.Where(x => x.AD_Username.Trim() == user_name.Trim()).Count() > 0;
        //     //if (!studentExists)
        //     //{
        //     //    throw new ResourceNotFoundException() { ExceptionMessage = "The Account was not found." };
        //     //}
            
        //     //OR A STORED PROCEDURE TO DELETE?
        //     //var result = RawSqlQuery<ScheduleViewModel>.query("DELETE_SCHEDULE_BY_ID @ID_NUM", id);
       
        //     // Create iterable list to hold the stuff we want
        //     List<ScheduleViewModel> stuff = new List<ScheduleViewModel>();

        //     var scheduleList = _unitOfWork.ScheduleRepository.Find(x => x.ID_NUM.Equals(id));

        //     foreach (var scheduleItem in scheduleList)
        //     {
        //         try
        //         {
        //             ScheduleViewModel vm = new ScheduleViewModel(scheduleItem);
        //             stuff.Add(vm);
        //             _unitOfWork.ScheduleRepository.Delete(scheduleItem);
        //         }
        //         catch
        //         {
        //             //Ignore issue, continue to iterate
        //         }
        //     }

        //     _unitOfWork.Save();

        //     return stuff.AsEnumerable<ScheduleViewModel>(); ;

        // }
    }

}