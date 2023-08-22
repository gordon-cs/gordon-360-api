using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Gordon360.Models.ViewModels;

public class EmergencyContactViewModel
{

    public static implicit operator EmergencyContactViewModel(EmergencyContact emrg)
    {
        EmergencyContactViewModel vm = new EmergencyContactViewModel()
        {
            APPID = emrg.APPID,
            SEQ_NUMBER = emrg.SEQ_NUM,
            AD_Username = emrg.AD_Username ?? "",
            LastName = emrg.lastname ?? "",
            FirstName = emrg.firstname ?? "",
            HomePhone = emrg.HomePhone ?? "",
            WorkPhone = emrg.WorkPhone ?? "",
            MobilePhone = emrg.MobilePhone ?? "",
            notes = emrg.notes ?? "",
            Relationship = emrg.relationship ?? "",
        };

        return vm;
    }

    public Nullable<int> APPID { get; set; }
    public int SEQ_NUMBER { get; set; }
    public string AD_Username { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string HomePhone { get; set; }
    public string WorkPhone { get; set; }
    public string MobilePhone { get; set; }
    public string notes { get; set; }
    public string Relationship { get; set; }
}
