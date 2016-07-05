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

        public int MEMBERSHIP_ID { get; set; }
        [Required]
        public string ACT_CDE { get; set; }
        [Required]
        public string SESSION_CDE { get; set; }
        [Required]
        public int ID_NUM { get; set; }
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

    }

    public class SUPERVISOR_Metadata
    {
        [Required]
        public int SUP_ID { get; set; }
        [Required]
        public int ID_NUM { get; set; }
        [Required]
        public string ACT_CDE { get; set; }
        public string USER_NAME { get; set; }
        public string JOB_NAME { get; set; }
        public Nullable<System.DateTime> JOB_TIME { get; set; }
    }

    public class JNZB_ACTIVITIES_Metadata
    {
        public int ENTRY_ID { get; set; }
        [Required]
        public string SESS_CDE { get; set; }
        [Required]
        public string ACT_CDE { get; set; }
        [Required]
        public int ID_NUM { get; set; }
        [Required]
        public string PART_CDE { get; set; }

        public bool MEMBERSHIP_STS { get; set; }
        public bool TRACK_MTG_ATTEND { get; set; }
        [Required]
        public System.DateTime BEGIN_DTE { get; set; }
        public System.DateTime END_DTE { get; set; }
        public string COMMENT_TXT { get; set; }
        public bool INCL_PROFILE_RPT { get; set; }
        public string USER_NAME { get; set; }
        public string USER_JOB { get; set; }
        public Nullable<System.DateTime> JOB_TIME { get; set; }
    }

    public class Request_Metadata
    {
        [Required]
        public string ACT_CDE { get; set; }
        [Required]
        public string ID_NUM { get; set; }
        [Required]
        public string PART_LVL { get; set; }
        [Required]
        public System.DateTime DATE_SENT { get; set; }
        public int REQUEST_ID { get; set; }
        public string COMMENT_TXT { get; set; }
        [Required]
        public string SESS_CDE { get; set; }
        // No need to require this because default is FALSE.
        public bool APPROVED { get; set; }
    }

    public class Activity_Info_Metadata
    {
        [Required]
        public string ACT_CDE { get; set; }
        public string ACT_DESCR { get; set; }
        public string MTG_DAY { get; set; }
        public Nullable<System.TimeSpan> MTG_TIME { get; set; }
        public string ACT_IMAGE { get; set; }
    }




}