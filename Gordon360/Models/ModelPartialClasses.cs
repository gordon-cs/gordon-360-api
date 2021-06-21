using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Gordon360.Models
{
    [MetadataType(typeof(Membership_Metadata))]
    public partial class Membership
    { }


    [MetadataType(typeof(SUPERVISOR_Metadata))]
    public partial class SUPERVISOR
    { }

    [MetadataType(typeof(Request_Metadata))]
    public partial class Request
    { }

    [MetadataType(typeof(Activity_Info_Metadata))]
    public partial class Activity_Info
    { }

}