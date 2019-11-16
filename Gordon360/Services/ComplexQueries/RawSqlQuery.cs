using Gordon360.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Services.ComplexQueries
{
    /// <summary>
    /// Helper class to execute Sql statements.
    /// </summary>
    /// <typeparam name="T">The class to which the result will be bound</typeparam>
    public static class RawSqlQuery<T> where T: class
    {
        /// <summary>
        /// Execute the sql query
        /// </summary>
        /// <param name="query">An sql statment. Can be a stored procedure or even a simple SELECT statment</param>
        /// <param name="parameters">Parameters to pass into the stored procedure</param>
        /// <returns></returns>
        public static IEnumerable<T> query(string query, params object[] parameters)
        {
            using (var context = new CCTEntities1())
            {
                var result = context.Database.SqlQuery<T>(query, parameters).AsEnumerable();
               return result.ToList();
            }
        }

        /// <summary>
        /// Execute the sql query on the StudentTimesheets database
        /// </summary>
        /// <param name="query">An sql statment. Can be a stored procedure or even a simple SELECT statment</param>
        /// <param name="parameters">Parameters to pass into the stored procedure</param>
        /// <returns></returns>
        public static IEnumerable<T> StudentTimesheetQuery(string query, params object[] parameters)
        {
            using (var context = new StudentTimesheetsEntities())
            {
                var result = context.Database.SqlQuery<T>(query, parameters).AsEnumerable();
                return result.ToList();
            }
        }
    }
}