﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT;

[Table("RA_On_Call", Schema = "Housing")]
public partial class RA_On_Call
{
    [Key]
    public int Record_ID { get; set; }

    public int Hall_ID { get; set; }

    public int Ra_ID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime Check_in_time { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Check_out_time { get; set; }

    [ForeignKey("Hall_ID")]
    [InverseProperty("RA_On_Call")]
    public virtual Halls Hall { get; set; }

    [ForeignKey("Ra_ID")]
    [InverseProperty("RA_On_Call")]
    public virtual Resident_Advisor Ra { get; set; }
}