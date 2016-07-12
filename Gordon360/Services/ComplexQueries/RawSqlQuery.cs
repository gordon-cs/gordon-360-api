using Gordon360.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Services.ComplexQueries
{
    public static class RawSqlQuery<T> where T: class
    {
        public static IEnumerable<T> query(string query, params object[] parameters)
        {
            using (var context = new CCTEntities())
            {
                var result = context.Database.SqlQuery<T>(query, parameters).AsEnumerable(); ;
                return result.ToList();
            }
        }
    }
}