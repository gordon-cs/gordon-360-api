using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT;

[Table("Missing", Schema = "LostAndFound")]
public partial class Missing
{
    [Key]
    public int recordID { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string firstName { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string lastName { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string category { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? brand { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string description { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string locationLost { get; set; }

    public bool? stolen { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? stolenDescription { get; set; }

    [Column(TypeName = "date")]
    public DateTime? dateLost { get; set; }

    [Column(TypeName = "date")]
    public DateTime? dateCreated { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string phoneNumber { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? altPhone { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string emailAddr { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string status { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? adminUsername { get; set; }
}
