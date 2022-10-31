﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT
{
    [Table("Participant", Schema = "RecIM")]
    public partial class Participant
    {
        public Participant()
        {
            MatchParticipant = new HashSet<MatchParticipant>();
            ParticipantActivity = new HashSet<ParticipantActivity>();
            ParticipantNotification = new HashSet<ParticipantNotification>();
            ParticipantStatusHistory = new HashSet<ParticipantStatusHistory>();
            ParticipantTeam = new HashSet<ParticipantTeam>();
        }

        [Key]
        public int ID { get; set; }

        [InverseProperty("Participant")]
        public virtual ICollection<MatchParticipant> MatchParticipant { get; set; }
        [InverseProperty("Participant")]
        public virtual ICollection<ParticipantActivity> ParticipantActivity { get; set; }
        [InverseProperty("Participant")]
        public virtual ICollection<ParticipantNotification> ParticipantNotification { get; set; }
        [InverseProperty("Participant")]
        public virtual ICollection<ParticipantStatusHistory> ParticipantStatusHistory { get; set; }
        [InverseProperty("Participant")]
        public virtual ICollection<ParticipantTeam> ParticipantTeam { get; set; }
    }
}