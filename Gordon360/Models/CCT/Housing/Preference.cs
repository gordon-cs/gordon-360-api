﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT;

[PrimaryKey("ApplicationID", "Preference1")]
[Table("Preference", Schema = "Housing")]
public partial class Preference
{
    public int RowID { get; set; }

    [Key]
    [StringLength(255)]
    [Unicode(false)]
    public string ApplicationID { get; set; }

    [Key]
    [Column("Preference")]
    [StringLength(255)]
    [Unicode(false)]
    public string Preference1 { get; set; }
}