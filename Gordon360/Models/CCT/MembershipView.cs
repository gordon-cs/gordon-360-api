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
    public partial class MembershipView
    {
        public int MembershipID { get; set; }
        [Required]
        [StringLength(8)]
        [Unicode(false)]
        public string ActivityCode { get; set; }
        [Required]
        [StringLength(45)]
        [Unicode(false)]
        public string ActivityDescription { get; set; }
        [Unicode(false)]
        public string ActivityImagePath { get; set; }
        [Required]
        [StringLength(8)]
        [Unicode(false)]
        public string SessionCode { get; set; }
        [StringLength(1000)]
        [Unicode(false)]
        public string SessionDescription { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string Username { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string FirstName { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string LastName { get; set; }
        [Required]
        [StringLength(5)]
        [Unicode(false)]
        public string Participation { get; set; }
        [Required]
        [StringLength(45)]
        [Unicode(false)]
        public string ParticipationDescription { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime StartDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EndDate { get; set; }
        [StringLength(45)]
        [Unicode(false)]
        public string Description { get; set; }
        public bool? GroupAdmin { get; set; }
        public bool? Privacy { get; set; }
    }
}