using System.ComponentModel.DataAnnotations;

namespace cct_api.models
{
    public class Activity
    {   
        public string activity_id { get; set; }
        [Required]
        [StringLengthAttribute(10, MinimumLength = 1)]
        public string activity_name { get; set; }
        [Required]
        [StringLengthAttribute(10, MinimumLength = 1)]
        public string activity_advisor { get; set; }
        [Required]
        [StringLengthAttribute(10, MinimumLength = 1)]
        public string activity_description { get; set; }
    }
}