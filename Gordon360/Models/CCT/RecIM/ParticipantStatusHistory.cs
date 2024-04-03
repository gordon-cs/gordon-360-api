﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT;

[Table("ParticipantStatusHistory", Schema = "RecIM")]
public partial class ParticipantStatusHistory
{
    [Key]
    public int ID { get; set; }

    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string ParticipantUsername { get; set; }

    public int StatusID { get; set; }

    [Column(TypeName = "date")]
    public DateTime StartDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime? EndDate { get; set; }

    [ForeignKey("ParticipantUsername")]
    [InverseProperty("ParticipantStatusHistory")]
    public virtual Participant ParticipantUsernameNavigation { get; set; }

    [ForeignKey("StatusID")]
    [InverseProperty("ParticipantStatusHistory")]
    public virtual ParticipantStatus Status { get; set; }
}