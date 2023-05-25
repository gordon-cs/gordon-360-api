﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT
{
    [Table("Surface", Schema = "RecIM")]
    public partial class Surface
    {
        public Surface()
        {
            Match = new HashSet<Match>();
            SeriesSurface = new HashSet<SeriesSurface>();
        }

        [Key]
        public int ID { get; set; }
        [Required]
        [StringLength(64)]
        [Unicode(false)]
        public string Name { get; set; }
        [Required]
        [StringLength(256)]
        [Unicode(false)]
        public string Description { get; set; }

        [InverseProperty("Surface")]
        public virtual ICollection<Match> Match { get; set; }
        [InverseProperty("Surface")]
        public virtual ICollection<SeriesSurface> SeriesSurface { get; set; }
    }
}