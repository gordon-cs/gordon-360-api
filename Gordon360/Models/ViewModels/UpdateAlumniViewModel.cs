using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gordon360.Models.ViewModels
{
    public class UpdateAlumniViewModel
    {
        public int ID { get; set; }

        public int ID_NUM { get; set; }

        public string EMAIL { get; set; }

        public string HOME_PHONE { get; set; }
        
        public string MOBILE_PHONE { get; set; }
        
        public string ADDRESS_1 { get; set; }
        
        public string ADDRESS_2 { get; set; }
        
        public string CITY { get; set; }

        public string STATE { get; set; }
    }
}
