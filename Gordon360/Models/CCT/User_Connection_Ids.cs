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
    public partial class User_Connection_Ids
    {
        [Required]
        [StringLength(72)]
        [Unicode(false)]
        public string user_id { get; set; }
        [Required]
        [StringLength(72)]
        [Unicode(false)]
        public string connection_id { get; set; }
    }
}