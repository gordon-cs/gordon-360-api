﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gordon360.Models.CCT
{
    public partial class NEWS_NOT_EXPIREDResult
    {
        public int SNID { get; set; }
        public string ADUN { get; set; }
        public int categoryID { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Image { get; set; }
        public bool? Sent { get; set; }
        public bool? thisPastMailing { get; set; }
        public DateTime? Entered { get; set; }
        public string categoryName { get; set; }
        public int? SortOrder { get; set; }
        public DateTime? ManualExpirationDate { get; set; }
    }
}
