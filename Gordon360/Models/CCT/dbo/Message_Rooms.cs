﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT
{
    [Table("Message_Rooms", Schema = "dbo")]
    public partial class Message_Rooms
    {
        [Key]
        public int room_id { get; set; }
        [Required]
        [StringLength(72)]
        [Unicode(false)]
        public string name { get; set; }
        public bool group { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime createdAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime lastUpdated { get; set; }
        public byte[] roomImage { get; set; }
    }
}