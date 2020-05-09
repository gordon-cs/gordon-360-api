using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class StudentNewsViewModel
    {
        public int SNID { get; set; }
        public string ADUN { get; set; }
        public int categoryID { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool? Accepted { get; set; }
        public bool? Sent { get; set; }
        public bool? thisPastMailing { get; set; }
        public Nullable<DateTime> Entered { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string categoryName { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public Nullable<DateTime> ManualExpirationDate { get; set; }

    }
}