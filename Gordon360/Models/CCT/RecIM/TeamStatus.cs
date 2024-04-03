﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT;

[Table("TeamStatus", Schema = "RecIM")]
public partial class TeamStatus
{
    [Key]
    public int ID { get; set; }

    [Required]
    [StringLength(256)]
    [Unicode(false)]
    public string Description { get; set; }

    [InverseProperty("Status")]
    public virtual ICollection<Team> Team { get; set; } = new List<Team>();
}