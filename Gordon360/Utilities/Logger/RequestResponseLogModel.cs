using System;
using System.Collections.Generic;

namespace Gordon360.Utilities.Logger
{
    public class RequestResponseLogModel
    {
        public string LogId { get; set; }           /*Guid.NewGuid().ToString()*/
        public string Node { get; set; }            /*project name*/
        public string ClientIp { get; set; }
        public string TraceId { get; set; }         /*HttpContext TraceIdentifier*/


        public DateTime? RequestDateTimeUtc { get; set; }
        public DateTime? RequestDateTimeUtcActionLevel { get; set; }
        public string RequestPath { get; set; }
        public string RequestQuery { get; set; }
        public List<KeyValuePair<string, string>> RequestQueries { get; set; }
        public string RequestMethod { get; set; }
        public string RequestScheme { get; set; }
        public string RequestHost { get; set; }
        public Dictionary<string, string> RequestHeaders { get; set; }
        public string RequestBody { get; set; }
        public string RequestContentType { get; set; }


        public DateTime? ResponseDateTimeUtc { get; set; }
        public DateTime? ResponseDateTimeUtcActionLevel { get; set; }
        public string ResponseStatus { get; set; }
        public Dictionary<string, string> ResponseHeaders { get; set; }
        // public string ResponseBody { get; set; }
        public string ResponseContentType { get; set; }


        public bool? IsExceptionActionLevel { get; set; }
        public string ExceptionMessage { get; set; }
        public string ExceptionStackTrace { get; set; }

        public RequestResponseLogModel()
        {
            LogId = Guid.NewGuid().ToString();
        }
    }
}
