﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT
{
    [Keyless]
    public partial class FacStaff
    {
        [Required]
        [StringLength(10)]
        [Unicode(false)]
        public string ID { get; set; }
        [StringLength(5)]
        [Unicode(false)]
        public string Title { get; set; }
        [StringLength(20)]
        [Unicode(false)]
        public string FirstName { get; set; }
        [StringLength(20)]
        [Unicode(false)]
        public string MiddleName { get; set; }
        [StringLength(25)]
        [Unicode(false)]
        public string LastName { get; set; }
        [StringLength(3)]
        [Unicode(false)]
        public string Suffix { get; set; }
        [StringLength(15)]
        [Unicode(false)]
        public string MaidenName { get; set; }
        [StringLength(20)]
        [Unicode(false)]
        public string Nickname { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string OnCampusDepartment { get; set; }
        [StringLength(10)]
        [Unicode(false)]
        public string OnCampusBuilding { get; set; }
        [StringLength(10)]
        [Unicode(false)]
        public string OnCampusRoom { get; set; }
        [StringLength(10)]
        [Unicode(false)]
        public string OnCampusPhone { get; set; }
        [StringLength(10)]
        [Unicode(false)]
        public string OnCampusPrivatePhone { get; set; }
        [StringLength(1)]
        [Unicode(false)]
        public string OnCampusFax { get; set; }
        [StringLength(60)]
        [Unicode(false)]
        public string HomeStreet1 { get; set; }
        [StringLength(60)]
        [Unicode(false)]
        public string HomeStreet2 { get; set; }
        [StringLength(30)]
        [Unicode(false)]
        public string HomeCity { get; set; }
        [StringLength(2)]
        [Unicode(false)]
        public string HomeState { get; set; }
        [StringLength(10)]
        [Unicode(false)]
        public string HomePostalCode { get; set; }
        [StringLength(3)]
        [Unicode(false)]
        public string HomeCountry { get; set; }
        [StringLength(15)]
        [Unicode(false)]
        public string HomePhone { get; set; }
        [StringLength(1)]
        [Unicode(false)]
        public string HomeFax { get; set; }
        [Required]
        [StringLength(1)]
        [Unicode(false)]
        public string KeepPrivate { get; set; }
        [StringLength(100)]
        [Unicode(false)]
        public string JobTitle { get; set; }
        [StringLength(20)]
        [Unicode(false)]
        public string SpouseName { get; set; }
        [StringLength(3)]
        [Unicode(false)]
        public string Dept { get; set; }
        [StringLength(14)]
        [Unicode(false)]
        public string Barcode { get; set; }
        [StringLength(1)]
        [Unicode(false)]
        public string Gender { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string Email { get; set; }
        public int? ActiveAccount { get; set; }
        [StringLength(7)]
        [Unicode(false)]
        public string Type { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string AD_Username { get; set; }
        [StringLength(75)]
        [Unicode(false)]
        public string office_hours { get; set; }
        public int? preferred_photo { get; set; }
        public int? show_pic { get; set; }
        [StringLength(45)]
        [Unicode(false)]
        public string BuildingDescription { get; set; }
        [StringLength(63)]
        [Unicode(false)]
        public string Country { get; set; }
        [StringLength(20)]
        [Unicode(false)]
        public string Mail_Location { get; set; }
        [StringLength(100)]
        [Unicode(false)]
        public string Mail_Description { get; set; }
    }
}