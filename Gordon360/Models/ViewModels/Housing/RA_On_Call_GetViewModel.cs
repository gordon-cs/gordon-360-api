using Gordon360.Extensions.System;
using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels;
public class RA_On_Call_GetViewModel
{
    public string Hall_ID { get; set; }
    public string Hall_Name { get; set; }
    public string Room_Number { get; set; }
    public string RA_Name { get; set; }
    public string Preferred_Contact { get; set; }
    public DateTime Check_In_Time { get; set; }
    public string RD_Email { get; set; }
    public string RA_UserName { get; set; }
    public string RD_UserName { get; set; }
    public string RD_Name { get; set; }
    public string RA_Photo { get; set; }
}