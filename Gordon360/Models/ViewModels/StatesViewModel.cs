using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.ViewModels
{
    public class StatesViewModel
    {
        [Key]
        [StringLength(63)]
        [Unicode(false)]
        public string Name {  get; set; }
        [Key]
        [StringLength(31)]
        [Unicode(false)]
        public string Abbreviation { get; set; }
    }
}
