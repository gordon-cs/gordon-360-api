using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

/* Place All Data Annotation and and Validation tags here 
 Model classes are automatically generated and so any code added
 to them will be remove once they are regenerated. 
 Because the generated Model classes are partial classes, we can 
 'extend' there definition here without worrying about the code being lost
 */

namespace Gordon360.Models
{

    public class Membership_Metadata
    {

        [Required]
        public string ACT_CDE { get; set; }
        [Required]
        public string SESSION_CDE { get; set; }
        [Required]
        public int ID_NUM { get; set; }
        [Required]
        public string PART_CDE { get; set; }
        [Required]
        public DateTime BEGIN_DTE { get; set; }


    }

    public class SUPERVISOR_Metadata
    {

        [Required]
        public int ID_NUM { get; set; }
        [Required]
        public string ACT_CDE { get; set; }
        [Required]
        public string SESS_CDE { get; set; }
    }


    public class Request_Metadata
    {
        [Required]
        public string ACT_CDE { get; set; }
        [Required]
        public int ID_NUM { get; set; }
        [Required]
        public string PART_CDE { get; set; }
        [Required]
        public DateTime DATE_SENT { get; set; }
        [Required]
        public string SESS_CDE { get; set; }
    }

    public class Activity_Info_Metadata
    {
        [Required]
        public string ACT_CDE { get; set; }
    }




}