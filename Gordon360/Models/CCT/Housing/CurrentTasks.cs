﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT;

[Keyless]
public partial class CurrentTasks
{
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

    [Column(TypeName = "datetime")]
    public DateTime OccurDate { get; set; }

    public bool IsComplete { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string CompletedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CompletedDate { get; set; }
}