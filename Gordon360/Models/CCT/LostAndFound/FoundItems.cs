﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT;

[Table("FoundItems", Schema = "LostAndFound")]
public partial class FoundItems
{
    [Key]
    public int ID { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string DESCRIPTION { get; set; }
}
