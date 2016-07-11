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
        public string IDNumber { get; set; }
        public string ActivityCode { get; set; }
        public string ActivityDescription { get; set; }
        public string SessionCode { get; set; }
        public string SessionDescription { get; set; }

        public static implicit operator SupervisorViewModel(SUPERVISOR sup)
        {
            SupervisorViewModel vm = new SupervisorViewModel
            {
                SupervisorID = sup.SUP_ID,
                IDNumber = sup.ID_NUM.Trim(),
                ActivityCode = sup.ACT_CDE.Trim(),
                SessionCode = sup.SESSION_CDE.Trim()

            };

            return vm;
        }
    }

}