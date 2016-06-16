using System.ComponentModel.DataAnnotations;

namespace cct_api.models
{
    public class Activity
    {   
        [RequiredAttribute]    
        public string activity_id { get; set; }
        [RequiredAttribute]
        public string activity_name { get; set; }
        [RequiredAttribute]
        public string activity_advisor { get; set; }
        [RequiredAttribute]
        public string activity_description { get; set; }
    }
}