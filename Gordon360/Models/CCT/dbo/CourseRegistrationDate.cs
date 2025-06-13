using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gordon360.Models.CCT
{
    [Keyless]
    [Table("CourseRegistrationDates", Schema = "dbo")]
    public class CourseRegistrationDate
    {
        [Column("ID_NUM")]
        public int StudentId { get; set; }

        [Required]
        [Column("AD_Username")]
        public string Username { get; set; }

        [Column("YR_TRM_DESC")]
        public string TermDescription { get; set; }

        [Column("ADD_BEG_DTE")]
        public DateTime? StartDate { get; set; }

        [Column("ADD_END_DTE")]
        public DateTime? EndDate { get; set; }

    }
}
