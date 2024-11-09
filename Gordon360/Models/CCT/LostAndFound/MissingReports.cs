﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT;

[Table("MissingReports", Schema = "LostAndFound")]
public partial class MissingReports
{
    [Key]
    public int ID { get; set; }

    [Required]
    [StringLength(255)]
    [Unicode(false)]
    public string submitterID { get; set; }

    public bool forGuest { get; set; }

    [Required]
    [StringLength(255)]
    [Unicode(false)]
    public string category { get; set; }

    [Required]
    [StringLength(255)]
    [Unicode(false)]
    public string colors { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string brand { get; set; }

    [Required]
    [StringLength(255)]
    [Unicode(false)]
    public string description { get; set; }

    [Required]
    [StringLength(255)]
    [Unicode(false)]
    public string locationLost { get; set; }

    public bool stolen { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string stolenDesc { get; set; }

    [Column(TypeName = "date")]
    public DateTime dateLost { get; set; }

    [Column(TypeName = "date")]
    public DateTime dateCreated { get; set; }

    [Required]
    [StringLength(255)]
    [Unicode(false)]
    public string status { get; set; }

    [InverseProperty("missing")]
    public virtual ICollection<ActionsTaken> ActionsTaken { get; set; } = new List<ActionsTaken>();

    [InverseProperty("missing")]
    public virtual ICollection<GuestUsers> GuestUsers { get; set; } = new List<GuestUsers>();

    [ForeignKey("category")]
    [InverseProperty("MissingReports")]
    public virtual Categories categoryNavigation { get; set; }

    [ForeignKey("status")]
    [InverseProperty("MissingReports")]
    public virtual Statuses statusNavigation { get; set; }
}