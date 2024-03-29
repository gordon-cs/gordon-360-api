﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT;

[Table("MatchParticipant", Schema = "RecIM")]
public partial class MatchParticipant
{
    [Key]
    public int ID { get; set; }

    public int MatchID { get; set; }

    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string ParticipantUsername { get; set; }

    public int TeamID { get; set; }

    [ForeignKey("MatchID")]
    [InverseProperty("MatchParticipant")]
    public virtual Match Match { get; set; }

    [ForeignKey("ParticipantUsername")]
    [InverseProperty("MatchParticipant")]
    public virtual Participant ParticipantUsernameNavigation { get; set; }

    [ForeignKey("TeamID")]
    [InverseProperty("MatchParticipant")]
    public virtual Team Team { get; set; }
}