﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT;

[Table("CustomParticipant", Schema = "RecIM")]
public partial class CustomParticipant
{
    public int ID { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Username { get; set; }

    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string Email { get; set; }

    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string FirstName { get; set; }

    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string LastName { get; set; }

    [ForeignKey("Username")]
    [InverseProperty("CustomParticipant")]
    public virtual Participant UsernameNavigation { get; set; }
}