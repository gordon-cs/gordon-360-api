using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCT_App.Models.ViewModels
{
    public class StaffViewModel
    {
        public string StaffID { get; set; }
        public string StaffName { get; set; }
        public string StaffEmail { get; set; }
        public string UserName { get; set; }

        public static implicit operator StaffViewModel(Staff staff)
        {
            StaffViewModel vm = new StaffViewModel
            {
                StaffID = staff.staff_id,
                StaffName = staff.staff_name,
                StaffEmail = staff.staff_email ?? "",
                UserName = staff.user_name ?? ""
            };

            return vm;
        }
    }
}