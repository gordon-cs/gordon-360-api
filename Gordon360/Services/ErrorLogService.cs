using System;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Models.CCT;
using Gordon360.Database.CCT;

namespace Gordon360.Services
{
    /// <summary>
    /// Service Class that facilitates data transactions between the ErrorLogController and the ERROR_LOG database model.
    /// </summary>
    public class ErrorLogService : IErrorLogService
    {
        private CCTContext _context;

        public ErrorLogService(CCTContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new error log to storage.
        /// </summary>
        /// <param name="error_log">The error log to be added</param>
        /// <returns>The newly added error_log object</returns>
        public ERROR_LOG Add(ERROR_LOG error_log)
        {

            // The Add() method returns the added error_log.
            var payload = _context.ERROR_LOG.Add(error_log);

            if (payload == null)
            {
                throw new ResourceCreationException() { ExceptionMessage = "There was an error creating the error_log. Perhaps the error is a duplicate." };
            }
            _context.SaveChanges();

            return error_log;

        }

        /// <summary>
        /// Adds a new error log to storage, after creating the timestamp.
        /// </summary>
        /// <param name="error_message">The error message for the error log to be added</param>
        /// <returns>The newly added error_log object</returns>
        public ERROR_LOG Log(string error_message)
        {
            if (error_message == null)
            {
                throw new ResourceCreationException() { ExceptionMessage = "The error message was blank." };
            }

            DateTime time = DateTime.Now;

            ERROR_LOG error_log = new ERROR_LOG
            {
                LOG_TIME = time,
                LOG_MESSAGE = error_message,
            };

            // The Add() method returns the added error_log.
            var payload = _context.ERROR_LOG.Add(error_log);

            if (payload == null)
            {
                throw new ResourceCreationException() { ExceptionMessage = "There was an error creating the error_log. Perhaps the error is a duplicate." };
            }
            _context.SaveChanges();

            return error_log;

        }
    }
}