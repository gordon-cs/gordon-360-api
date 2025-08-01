﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT;

[Keyless]
public partial class ACCOUNT
{
    public int account_id { get; set; }

    [Required]
    [StringLength(10)]
    [Unicode(false)]
    public string gordon_id { get; set; }

    [StringLength(14)]
    [Unicode(false)]
    public string barcode { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string firstname { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string lastname { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string email { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string AD_Username { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string account_type { get; set; }

    [StringLength(8000)]
    [Unicode(false)]
    public string office_hours { get; set; }

    public int Primary_Photo { get; set; }

    public int Preferred_Photo { get; set; }

    public int show_pic { get; set; }

    public int Private { get; set; }

    public int ReadOnly { get; set; }

    public int is_police { get; set; }

    public int? Chapel_Required { get; set; }

    [Required]
    [StringLength(20)]
    [Unicode(false)]
    public string Mail_Location { get; set; }

    public int? Chapel_Attended { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Birth_Date { get; set; }
}