﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT
{
    [Table("Affiliation", Schema = "RecIM")]
    public partial class Affiliation
    {
        public Affiliation()
        {
            AffiliationPoints = new HashSet<AffiliationPoints>();
            Team = new HashSet<Team>();
        }

        [Key]
        [StringLength(50)]
        [Unicode(false)]
        public string Name { get; set; }
        [StringLength(128)]
        [Unicode(false)]
        public string Logo { get; set; }

        [InverseProperty("AffiliationNameNavigation")]
        public virtual ICollection<AffiliationPoints> AffiliationPoints { get; set; }
        [InverseProperty("AffiliationNavigation")]
        public virtual ICollection<Team> Team { get; set; }
    }
}