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
    /// Service Class that facilitates data transactions between the SchedulesController and the Schedule part of the database model.
    /// </summary>
    public class ScheduleService : IScheduleService
    {
        private IUnitOfWork _unitOfWork;

        public ScheduleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //ScheduleViewModel Get(string id) { }
        //IEnumerable<ScheduleViewModel> GetAll() { }
        //IEnumerable<ScheduleViewModel> GetScheduleEventsForStudent(string id) { }




        /// <summary>
        /// Fetch the schedule item whose id is specified by the parameter
        /// </summary>
        /// <param name="id">The schedule id</param>
        /// <returns>ScheduleViewModel if found, null if not found</returns>
        public ScheduleViewModel Get(int id)
        {
            var query = _unitOfWork.ScheduleRepository.GetById(id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Schedule was not found." };
            }

            var idParam = new SqlParameter("@SCHEDULE_ID", id);
            var result = RawSqlQuery<ScheduleViewModel>.query("SCHEDULE_PER_SCHEDULE_ID @SCHEDULE_ID", idParam).FirstOrDefault();

            if (result == null)
            {
                return null;
            }
            // Getting rid of database-inherited whitespace
            result.ScheduleID = result.ScheduleID;
            result.IDNumber = result.IDNumber;
            result.StartTime = result.StartTime;
            result.EndTime = result.EndTime;
            result.Title = result.Title.Trim();
            result.ItemDescription = result.ItemDescription.Trim();

            return result;
        }

        /// <summary>
        /// Fetches all schedule records from storage for an id.
        /// </summary>
        /// <returns>ScheduleViewModel IEnumerable. If no records were found, an empty IEnumerable is returned.</returns>
        public IEnumerable<ScheduleViewModel> GetAllByID(string id)
        {

            var result = RawSqlQuery<ScheduleViewModel>.query("ALL_SCHEDULES_PER_ID");
            // Trimming database generated whitespace ._.
            var trimmedResult = result.Select(x =>
            {
                var trim = x;
                trim.ScheduleID = x.ScheduleID;
                trim.IDNumber = x.IDNumber;
                trim.StartTime = x.StartTime;
                trim.EndTime = x.EndTime;
                trim.Title = x.Title.Trim();
                trim.ItemDescription = x.ItemDescription.Trim();
                return trim;
            });
            return trimmedResult;
        }

        /// <summary>
        /// Adds a new Schedule record to storage. Since we can't establish foreign key constraints and relationships on the database side,
        /// we might do it here by using something like the validateMembership() method.
        /// </summary>
        /// <param name="schedule">The membership to be added</param>
        /// <returns>The newly added Schedule object</returns>
        public SCHEDULE Add(SCHEDULE schedule)
        {
            // validate returns a boolean value.
            //validateMembership(membership);

            // The Add() method returns the added membership.
            var payload = _unitOfWork.ScheduleRepository.Add(schedule);

            // There is a unique constraint in the Database on columns (ID_NUM, BEGIN_DTE, END_DTE and DSCRIPT_TXT)
            if (payload == null)
            {
                throw new ResourceCreationException() { ExceptionMessage = "There was an error adding the schedule event. Verify that a similar schedule doesn't already exist." };
            }
            _unitOfWork.Save();

            return payload;

        }

        /// <summary>
        /// Delete the schedule whose id is specified by the parameter. Should we have a schedule id?
        /// </summary>
        /// <param name="id">The schedule id</param>
        /// <returns>The schedule that was just deleted</returns>
        public SCHEDULE Delete(int id)
        {
            var result = _unitOfWork.ScheduleRepository.GetById(id);
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Schedule was not found." };
            }
            result = _unitOfWork.ScheduleRepository.Delete(result);

            _unitOfWork.Save();

            return result;
        }

        /// <summary>
        /// Delete all schedule items for a student whose id is specified by the parameter. Should we have a schedule id?
        /// </summary>
        /// <param name="id">The student's id</param>
        /// <returns>The schedule that was just deleted</returns>
        public IEnumerable<ScheduleViewModel> DeleteAllForID(string id)
        {
            // Confirm that student exists
            //var studentExists = _unitOfWork.AccountRepository.Where(x => x.AD_Username.Trim() == user_name.Trim()).Count() > 0;
            //if (!studentExists)
            //{
            //    throw new ResourceNotFoundException() { ExceptionMessage = "The Account was not found." };
            //}
            
            //OR A STORED PROCEDURE TO DELETE?
            //var result = RawSqlQuery<ScheduleViewModel>.query("DELETE_SCHEDULE_BY_ID @ID_NUM", id);
       
            // Create iterable list to hold the stuff we want
            List<ScheduleViewModel> stuff = new List<ScheduleViewModel>();

            var scheduleList = _unitOfWork.ScheduleRepository.Find(x => x.ID_NUM.Equals(id));

            foreach (var scheduleItem in scheduleList)
            {
                try
                {
                    ScheduleViewModel vm = new ScheduleViewModel(scheduleItem);
                    stuff.Add(vm);
                    _unitOfWork.ScheduleRepository.Delete(scheduleItem);
                }
                catch
                {
                    //Ignore issue, continue to iterate
                }
            }

            _unitOfWork.Save();

            return stuff.AsEnumerable<ScheduleViewModel>(); ;

        }
    }

}