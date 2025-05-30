﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT;

[Keyless]
public partial class FoundItemData
{
    [Required]
    [StringLength(10)]
    [Unicode(false)]
    public string ID { get; set; }

    [Required]
    [StringLength(15)]
    [Unicode(false)]
    public string adminID { get; set; }

    public int? matchingMissingID { get; set; }

    [Required]
    [StringLength(255)]
    [Unicode(false)]
    public string category { get; set; }

    [Required]
    [StringLength(255)]
    [Unicode(false)]
    public string colors { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string brand { get; set; }

    [Required]
    [StringLength(511)]
    [Unicode(false)]
    public string description { get; set; }

    [Required]
    [StringLength(255)]
    [Unicode(false)]
    public string locationFound { get; set; }

    [Column(TypeName = "date")]
    public DateTime dateFound { get; set; }

    [Column(TypeName = "date")]
    public DateTime dateCreated { get; set; }

    [StringLength(63)]
    [Unicode(false)]
    public string foundByID { get; set; }

    public int? foundByGuestID { get; set; }

    public bool finderWants { get; set; }

    [StringLength(63)]
    [Unicode(false)]
    public string ownerID { get; set; }

    public int? guestOwnerID { get; set; }

    [Required]
    [StringLength(255)]
    [Unicode(false)]
    public string status { get; set; }

    [Required]
    [StringLength(255)]
    [Unicode(false)]
    public string storageLocation { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string submitterUsername { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string finderUsername { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string ownerUsername { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string finderFirstName { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string finderLastName { get; set; }

    [StringLength(15)]
    [Unicode(false)]
    public string finderPhone { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string finderEmail { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string ownerFirstName { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string ownerLastName { get; set; }

    [StringLength(15)]
    [Unicode(false)]
    public string ownerPhone { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string ownerEmail { get; set; }
}