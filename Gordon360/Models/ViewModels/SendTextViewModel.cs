using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class SendTextViewModel
    {
        public String id { get; set; }

        public String room_id { get; set; }

        public String text { get; set; }

        public String createdAt { get; set; }

        public String image { get; set; }

        public String video { get; set; }

        public String audio { get; set; }

        public bool system { get; set; }

        public bool sent { get; set; }

        public bool received { get; set; }

        public bool pending { get; set; }

        public List<string> users_ids { get; set; }

        public string groupName { get; set; }

        public string groupText { get; set; }
    }
}