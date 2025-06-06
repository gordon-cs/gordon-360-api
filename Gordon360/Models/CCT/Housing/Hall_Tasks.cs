﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT;

[Table("Hall_Tasks", Schema = "Housing")]
public partial class Hall_Tasks
{
    [Key]
    public int Task_ID { get; set; }

    [Required]
    [StringLength(255)]
    [Unicode(false)]
    public string Name { get; set; }

    [Unicode(false)]
    public string Description { get; set; }

    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string Hall_ID { get; set; }

    public bool Is_Recurring { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Frequency { get; set; }

    public int? Interval { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime Start_Date { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? End_Date { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime Created_Date { get; set; }

    [InverseProperty("Task")]
    public virtual ICollection<Hall_Task_Occurrence> Hall_Task_Occurrence { get; set; } = new List<Hall_Task_Occurrence>();
}