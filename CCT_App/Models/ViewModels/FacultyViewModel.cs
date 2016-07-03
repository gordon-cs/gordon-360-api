using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCT_App.Models.ViewModels
{
    public class FacultyViewModel
    {
        public string FacultyID { get; set; }
        public string FacultyName { get; set; }
        public string FacultyEmail { get; set; }
        public string UserName { get; set; }

        public static implicit operator FacultyViewModel(Faculty f)
        {
            FacultyViewModel vm = new FacultyViewModel
            {
                FacultyID = f.faculty_id.Trim(),
                FacultyName = f.faculty_name.Trim(),
                FacultyEmail = f.faculty_email ?? "", // Some random records have null emails.
                UserName = f.user_name.Trim()
            };

            return vm;
        }
    }
}