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
    public partial class Alumni
    {
        [Required]
        [StringLength(25)]
        [Unicode(false)]
        public string ID { get; set; }
        [StringLength(25)]
        [Unicode(false)]
        public string WebUpdate { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string Title { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string FirstName { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string MiddleName { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string LastName { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string Suffix { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string MaidenName { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string NickName { get; set; }
        [StringLength(60)]
        [Unicode(false)]
        public string HomeStreet1 { get; set; }
        [StringLength(60)]
        [Unicode(false)]
        public string HomeStreet2 { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string HomeCity { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string HomeState { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string HomePostalCode { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string HomeCountry { get; set; }
        [StringLength(41)]
        [Unicode(false)]
        public string HomePhone { get; set; }
        [StringLength(25)]
        [Unicode(false)]
        public string HomeFax { get; set; }
        [StringLength(255)]
        [Unicode(false)]
        public string HomeEmail { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string JobTitle { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string MaritalStatus { get; set; }
        [StringLength(60)]
        [Unicode(false)]
        public string SpouseName { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string College { get; set; }
        [StringLength(25)]
        [Unicode(false)]
        public string ClassYear { get; set; }
        [StringLength(25)]
        [Unicode(false)]
        public string PreferredClassYear { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string Major1 { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string Major2 { get; set; }
        [StringLength(25)]
        [Unicode(false)]
        public string ShareName { get; set; }
        [StringLength(25)]
        [Unicode(false)]
        public string ShareAddress { get; set; }
        [StringLength(10)]
        [Unicode(false)]
        public string Gender { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string GradDate { get; set; }
        [StringLength(255)]
        [Unicode(false)]
        public string Email { get; set; }
        [StringLength(1)]
        [Unicode(false)]
        public string grad_student { get; set; }
        [StringLength(15)]
        [Unicode(false)]
        public string Barcode { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string AD_Username { get; set; }
        public int? show_pic { get; set; }
        public int? preferred_photo { get; set; }
        [StringLength(63)]
        [Unicode(false)]
        public string Country { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string Major2Description { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string Major1Description { get; set; }
    }
}