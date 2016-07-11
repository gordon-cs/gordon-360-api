using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class StudentViewModel
    {
        public string StudentID { get; set; }
        public string StudentName { get; set; }
        public string StudentEmail { get; set; }
        public string UserName { get; set; }


        public static implicit operator StudentViewModel(Student stu)
        {
            StudentViewModel vm = new StudentViewModel
            {
                StudentID = stu.student_id.Trim(),
                StudentName = stu.student_name.Trim(),
                UserName = stu.user_name ?? "", // Just in case some random record has a null user_name 
                StudentEmail = stu.student_email ?? "" // Just in case some random record has a null email field
            };

            return vm;
        }
    }
}