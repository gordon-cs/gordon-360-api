using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Gordon360.Models.ViewModels
{
    public class EmergencyContactViewModel
    {
        public EmergencyContactViewModel(EmergencyContact emrg)
        {
            APPID = emrg.APPID;
            AD_Username = emrg.AD_Username ?? "";
            LastName = emrg.lastname ?? "";
            FirstName = emrg.firstname ?? "";
            HomePhone = emrg.HomePhone ?? "";
            WorkPhone = emrg.WorkPhone ?? "";
            MobilePhone = emrg.MobilePhone ?? "";
            Relationship = emrg.relationship ?? "";
        }

        public int APPID { get; set; }
        public string AD_Username { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string HomePhone { get; set; }
        public string WorkPhone { get; set; }
        public string MobilePhone { get; set; }
        public string Relationship { get; set; }
    }
}