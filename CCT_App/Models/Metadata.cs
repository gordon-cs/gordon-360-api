using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

/* Place All Data Annotation and and Validation tags here 
 Model classes are automatically generated and so any code added
 to them will be remove once they are regenerated. 
 Because the generated Model classes are partial classes, we can 
 'extend' there definition here without worrying about the code being lost
 */

namespace CCT_App.Models
{
    
        public class Membership_Metadata
        {
            [Key]
            [Required]
            public int MEMBERSHIP_ID { get; set; }
            [ForeignKey("PART_DEF")]
            [Required]
            public string ACT_CDE { get; set; }
            [ForeignKey("CM_SESSION_MSTR")]
            [Required]
            public string SESSION_CDE { get; set; }
            [Required]
            [MinLength(3)]
            public string PART_LVL { get; set; }
            [Required]
            public System.DateTime BEGIN_DTE { get; set; }
            public Nullable<System.DateTime> END_DTE { get; set; }
            public string DESCRIPTION { get; set; }
            public string USER_NAME { get; set; }
            public string JOB_NAME { get; set; }
            public string JOB_TIME { get; set; }
            [Required]
            public int ID_NUM { get; set; }
    }

        public class SUPERVISOR_Metadata
        {
            [Key]
            [Required]
            public int SUP_ID { get; set; }
            [Required]
            public int ID_NUM { get; set; }
            [ForeignKey("PART_DEF")]
            [Required]
            public string ACT_CDE { get; set; }
            public string USER_NAME { get; set; }
            public string JOB_NAME { get; set; }
            public Nullable<System.DateTime> JOB_TIME { get; set; }
    }

        public class JNZB_ACTIVITIES_Metadata
        {
            [Key]
            [Required]
            public int ENTRY_ID { get; set; }
            [ForeignKey("CM_SESSION_MSTR")]
            [Required]
            public string SESS_CDE { get; set; }
            [ForeignKey("PART_DEF")]
            [Required]
            public string ACT_CDE { get; set; }
            [Required]
            public int ID_NUM { get; set; }
            public string PART_CDE { get; set; }
            [ForeignKey("Membership")]
            [Required]
            public bool MEMBERSHIP_STS { get; set; }
            public bool TRACK_MTG_ATTEND { get; set; }
            [Required]
            public System.DateTime BEGIN_DTE { get; set; }
            public System.DateTime END_DTE { get; set; }
            public string COMMENT_TXT { get; set; }
            [Required]
            public bool INCL_PROFILE_RPT { get; set; }
            public string USER_NAME { get; set; }
            public string USER_JOB { get; set; }
            public Nullable<System.DateTime> JOB_TIME { get; set; }
    }
        
}