using Gordon360.Extensions.System;
using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels;
    public class RA_ContactPreference
{
    public string RA_ID { get; set; } // ID of the RA
    public string Preferred_Contact_Method { get; set; } // e.g., "Phone", "Teams"
    public string Contact {  get; set; } // e.g "9788674500", "https..."
}
