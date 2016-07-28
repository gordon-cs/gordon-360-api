using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class SupervisorViewModel
    {
        public int SupervisorID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int IDNumber { get; set; }
        public string ActivityCode { get; set; }
        public string ActivityDescription { get; set; }
        public string ActivityImage { get; set; }
        public string ActivityImagePath { get; set; }
        public string SessionCode { get; set; }
        public string SessionDescription { get; set; }

        public static implicit operator SupervisorViewModel(SUPERVISOR sup)
        {
            SupervisorViewModel vm = new SupervisorViewModel
            {
                SupervisorID = sup.SUP_ID,
                IDNumber = sup.ID_NUM,
                ActivityCode = sup.ACT_CDE.Trim(),
                SessionCode = sup.SESS_CDE.Trim()

            };

            return vm;
        }
    }

}