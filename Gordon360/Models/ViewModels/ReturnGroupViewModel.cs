using System;
using System.Collections.Generic;
using System.Linq;

namespace Gordon360.Models.ViewModels
{
    public class ReturnGroupViewModel
    {
            public int id { get; set; }

            public String name { get; set; }

            public bool group { get; set; }

            public Byte[] image { get; set; }

            public List<String> usernames = new List<string>(500);

            public List<UserViewModel> users = new List<UserViewModel>(500);

            public SendTextViewModel message { get; set; }

            public DateTime createdAt { get; set; }

            public DateTime lastUpdated { get; set; }
    }
}