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
    public partial class Housing_HallChoices
    {
        public int HousingAppID { get; set; }
        public int Ranking { get; set; }
        [Required]
        [StringLength(15)]
        [Unicode(false)]
        public string HallName { get; set; }

        [ForeignKey(nameof(HousingAppID))]
        public virtual Housing_Applications HousingApp { get; set; }
    }
}