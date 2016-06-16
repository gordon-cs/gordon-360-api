using System.ComponentModel.DataAnnotations;

namespace cct_api.models
{
    public class Membership
    {   
        [RequiredAttribute]    
        public string membership_id { get; set; }
        [RequiredAttribute]
        public string student_id { get; set; }
        [RequiredAttribute]
        public string activity_id { get; set; }
        [RequiredAttribute]
        public string membership_level { get; set; }
    }
}