using Gordon360.Extensions.System;
using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels;
public class RA_On_CallViewModel
{
    public List<string> Hall_ID  { get; set; } //Hall ID(s) to check into
    public string Ra_ID { get; set; } // ID of ra checking in
}