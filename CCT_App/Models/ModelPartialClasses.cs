using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CCT_App.Models
{
    [MetadataType(typeof(Membership_Metadata))]
    public partial class Membership
    { }

    [MetadataType(typeof(JNZB_ACTIVITIES_Metadata))]
    public partial class JNZB_ACTIVITIES
    { }

    [MetadataType(typeof(SUPERVISOR_Metadata))]
    public partial class SUPERVISOR
    { }
   
}