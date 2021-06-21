using System;
using System.Collections.Generic;
using System.Linq;

namespace Gordon360.Models.ViewModels
{
    public class ReturnMessageViewModel
    {
        public String message_id { get; set; }

        public String text { get; set; }

        public DateTime createdAt { get; set; }

        public String user_id { get; set; }

        public Byte[] image { get; set; }

        public String video { get; set; }

        public String audio { get; set; }

        public bool system { get; set; }

        public bool received { get; set; }

        public bool pending { get; set; }

        public UserViewModel user { get; set; }

    }
}