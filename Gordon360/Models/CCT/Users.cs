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
    [Table("Users", Schema = "dbo")]
    public partial class Users
    {
        [Required]
        [StringLength(72)]
        [Unicode(false)]
        public string _id { get; set; }
        [Required]
        [StringLength(72)]
        [Unicode(false)]
        public string name { get; set; }
        public byte[] avatar { get; set; }
    }
}