/********************************
 * This file is manually created and may be edited
 * The view model allows access to all of the model's data without
 * anything unnecessary. It prevents a self-referencing loop error
 * in models that need a category that need a model etc.
 * The implicit operator allows conversion between the model and the view model
 ********************************/

using Gordon360.Models.MyGordon;
using System;

namespace Gordon360.Models.ViewModels
{
    public class StudentNewsViewModel
    {
        public int SNID { get; set; }
        public string ADUN { get; set; }
        public int categoryID { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Image { get; set; }
        public bool Accepted { get; set; }
        public bool? Sent { get; set; }
        public bool? thisPastMailing { get; set; }
        public DateTime? Entered { get; set; }
        public string categoryName { get; set; }
        public int? SortOrder { get; set; }
        public DateTime? ManualExpirationDate { get; set; }

        public static implicit operator StudentNewsViewModel(StudentNews n)
        {
            StudentNewsViewModel vm = new StudentNewsViewModel
            {
                SNID = n.SNID,
                ADUN = n.ADUN,
                categoryID = n.categoryID,
                Subject = n.Subject,
                Body = n.Body,
                Image = n.Image,
                // should default to unapproved (if null)
                Accepted = n.Accepted ?? false,
                Sent = n.Sent,
                thisPastMailing = n.thisPastMailing,
                Entered = n.Entered,
                categoryName = n.category?.categoryName ?? "",
                SortOrder = n.category?.SortOrder ?? null,
                ManualExpirationDate = n.ManualExpirationDate,
            };

            return vm;
        }
    }
}