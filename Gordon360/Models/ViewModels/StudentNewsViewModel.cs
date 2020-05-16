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
        public bool? Sent { get; set; }
        public bool? thisPastMailing { get; set; }
        public Nullable<DateTime> Entered { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string categoryName { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public Nullable<DateTime> ManualExpirationDate { get; set; }

        public static implicit operator StudentNewsViewModel(NEWS_NEW_Result n)
        {
            StudentNewsViewModel vm = new StudentNewsViewModel
            {
                SNID = n.SNID,
                ADUN = n.ADUN,
                categoryID = n.categoryID,
                Subject = n.Subject,
                Body = n.Body,
                Sent = n.Sent,
                thisPastMailing = n.thisPastMailing,
                Entered = n.Entered,
                fname = n.fname,
                lname = n.lname,
                categoryName = n.categoryName,
                SortOrder = n.SortOrder,
                ManualExpirationDate = n.ManualExpirationDate,
            };

            return vm;
        }
    }
}