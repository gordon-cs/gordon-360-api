﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT
{
    public partial class Config
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string Key { get; set; }
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string Value { get; set; }
    }
}