using Gordon360.Extensions.System;
using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels;
    public class RA_ContactPreference
{
    public string Ra_ID { get; set; } // ID of the RA
    public string PreferredContactMethod { get; set; } // e.g., "Phone", "Teams"
}
