﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT
{
    [Table("ACT_INFO", Schema = "dbo")]
    public partial class ACT_INFO
    {
        [Key]
        [StringLength(8)]
        [Unicode(false)]
        public string ACT_CDE { get; set; }
        [Required]
        [StringLength(45)]
        [Unicode(false)]
        public string ACT_DESC { get; set; }
        [Unicode(false)]
        public string ACT_BLURB { get; set; }
        [Unicode(false)]
        public string ACT_URL { get; set; }
        [Unicode(false)]
        public string ACT_IMG_PATH { get; set; }
        [StringLength(3)]
        [Unicode(false)]
        public string ACT_TYPE { get; set; }
        [StringLength(60)]
        [Unicode(false)]
        public string ACT_TYPE_DESC { get; set; }
        public bool? PRIVACY { get; set; }
        [Unicode(false)]
        public string ACT_JOIN_INFO { get; set; }
    }
}