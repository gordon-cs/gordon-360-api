﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT
{
    public partial class Housing_Applicants
    {
        [Key]
        public int HousingAppID { get; set; }
        [Key]
        [StringLength(50)]
        [Unicode(false)]
        public string Username { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string AprtProgram { get; set; }
        public bool? AprtProgramCredit { get; set; }
        [Required]
        [StringLength(8)]
        [Unicode(false)]
        public string SESS_CDE { get; set; }

        [ForeignKey("HousingAppID")]
        [InverseProperty("Housing_Applicants")]
        public virtual Housing_Applications HousingApp { get; set; }
    }
}