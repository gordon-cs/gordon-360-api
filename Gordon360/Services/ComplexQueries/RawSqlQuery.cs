using Gordon360.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gordon360.Services.ComplexQueries
{
    ///// <summary>
    ///// Helper class to execute Sql statements.
    ///// </summary>
    ///// <typeparam name="T">The class to which the result will be bound</typeparam>
    //public static class RawSqlQuery<T> where T: class
    //{
    //    // TODO: Replace all RawSqlQuery stored procedure calls with a strongly typed call to the procedure through EF6

    //    /// <summary>
    //    /// Execute the sql query
    //    /// </summary>
    //    /// <param name="query">An sql statement. Can be a stored procedure or even a simple SELECT statement</param>
    //    /// <param name="parameters">Parameters to pass into the stored procedure</param>
    //    /// <returns></returns>
    //    public static IEnumerable<T> query(string query, params object[] parameters)
    //    {
    //        using (var context = new CCTContext("metadata = res://*/Models.CCT_DB_Models.csdl|res://*/Models.CCT_DB_Models.ssdl|res://*/Models.CCT_DB_Models.msl;provider=System.Data.SqlClient;provider connection string=\u0022data source=admintrainsql.gordon.edu;initial catalog=CCT;integrated security=True;multipleactiveresultsets=True;application name=EntityFramework\u0022)"))
    //        {
    //           var result = context.Database.SqlQuery<T>(query, parameters).AsEnumerable();
    //           return result.ToList();
    //        }
    //    }

    //    /// <summary>
    //    /// Execute the sql query on the StudentTimesheets database
    //    /// </summary>
    //    /// <param name="query">An sql statement. Can be a stored procedure or even a simple SELECT statment</param>
    //    /// <param name="parameters">Parameters to pass into the stored procedure</param>
    //    /// <returns></returns>
    //    public static IEnumerable<T> StudentTimesheetQuery(string query, params object[] parameters)
    //    {
    //        using (var context = new StudentTimesheetsEntities("metadata=res://*/Models.StudentTimesheet_DB_Models.csdl|res://*/Models.StudentTimesheet_DB_Models.ssdl|res://*/Models.StudentTimesheet_DB_Models.msl;provider=System.Data.SqlClient;provider connection string=\u0022data source=admintrainsql.gordon.edu;initial catalog=StudentTimesheets;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework\u0022"))
    //        {
    //            var result = context.Database.SqlQuery<T>(query, parameters).AsEnumerable();
    //            return result.ToList();
    //        }
    //    }

    //    /// <summary>
    //    /// Execute the sql query on the StaffTimesheets database
    //    /// </summary>
    //    /// <param name="query">An sql statement. Can be a stored procedure or even a simple SELECT statment</param>
    //    /// <param name="parameters">Parameters to pass into the stored procedure</param>
    //    /// <returns></returns>
    //    public static IEnumerable<T> StaffTimesheetQuery(string query, params object[] parameters)
    //    {
    //        using (var context = new StaffTimesheetsEntities("metadata=res://*/Models.StaffTimesheets_DB_Models.csdl|res://*/Models.StaffTimesheets_DB_Models.ssdl|res://*/Models.StaffTimesheets_DB_Models.msl;provider=System.Data.SqlClient;provider connection string=\u0022data source=admintrainsql.gordon.edu;initial catalog=StaffTimesheets;integrated security=True;multipleactiveresultsets=True;application name=EntityFramework\u0022"))
    //        {
    //            var result = context.Database.SqlQuery<T>(query, parameters).AsEnumerable();
    //            return result.ToList();
    //        }
    //    }
    //}
}


