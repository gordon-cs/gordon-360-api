﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT;

[Table("Information_Change_Request", Schema = "dbo")]
public partial class Information_Change_Request
{
    [Key]
    public long ID { get; set; }

    public long RequestNumber { get; set; }

    [Required]
    [StringLength(16)]
    [Unicode(false)]
    public string ID_Num { get; set; }

    [Required]
    [StringLength(20)]
    [Unicode(false)]
    public string FieldName { get; set; }

    [StringLength(128)]
    [Unicode(false)]
    public string FieldValue { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime TimeStamp { get; set; }
}