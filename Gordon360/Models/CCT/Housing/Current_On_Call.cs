﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT;

[Keyless]
public partial class Current_On_Call
{
    [Required]
    [StringLength(10)]
    [Unicode(false)]
    public string Hall_ID { get; set; }

    [Required]
    [StringLength(10)]
    [Unicode(false)]
    public string Ra_ID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime Check_in_time { get; set; }
}