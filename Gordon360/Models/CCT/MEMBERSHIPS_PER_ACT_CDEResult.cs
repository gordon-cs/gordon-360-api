﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gordon360.Models.CCT
{
    public partial class MEMBERSHIPS_PER_ACT_CDEResult
    {
        public int MembershipID { get; set; }
        public string ActivityCode { get; set; }
        public string ActivityDescription { get; set; }
        public string ActivityImagePath { get; set; }
        public string SessionCode { get; set; }
        public string SessionDescription { get; set; }
        public int IDNumber { get; set; }
        public string AD_Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mail_Location { get; set; }
        public string Participation { get; set; }
        public string ParticipationDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
        public bool? GroupAdmin { get; set; }
        public int AccountPrivate { get; set; }
    }
}
