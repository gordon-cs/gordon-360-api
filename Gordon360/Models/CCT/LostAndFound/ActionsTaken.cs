﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT;

[Table("ActionsTaken", Schema = "LostAndFound")]
public partial class ActionsTaken
{
    [Key]
    public int ID { get; set; }

    public int missingID { get; set; }

    [Required]
    [StringLength(255)]
    [Unicode(false)]
    public string action { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime actionDate { get; set; }

    [Required]
    [StringLength(255)]
    [Unicode(false)]
    public string actionNote { get; set; }

    [Required]
    [StringLength(255)]
    [Unicode(false)]
    public string submitterID { get; set; }

    public bool isPublic { get; set; }

    [ForeignKey("missingID")]
    [InverseProperty("ActionsTaken")]
    public virtual MissingReports missing { get; set; }
}