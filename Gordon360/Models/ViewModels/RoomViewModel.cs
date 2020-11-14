using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Models;
namespace Gordon360.Models.ViewModels
{
    public class RoomViewModel
    {
        public String room_id { get; set; }

        public String name { get; set; }

        public bool group { get; set; }

        public DateTime createdAt { get; set; }

        public DateTime lastUpdated { get; set; }

        public String roomImage { get; set; }

        public IEnumerable<UserViewModel> users { get; set; }
    }
}