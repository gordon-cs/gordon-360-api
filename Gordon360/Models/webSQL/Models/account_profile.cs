﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.webSQL.Models;

[Table("account_profile")]
public partial class account_profile
{
    [Key]
    public int account_id { get; set; }

    [StringLength(8000)]
    [Unicode(false)]
    public string office_hours { get; set; }
}