using System;
using System.Collections.Generic;
using System.Linq;
using Gordon360.Static.Names;
namespace Gordon360.Models.ViewModels
{
    public class ProfileCustomViewModel
    {
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string LinkedIn { get; set; }
        public string Handshake { get; set; }

        public static implicit operator ProfileCustomViewModel(CUSTOM_PROFILE pro)
        {
            ProfileCustomViewModel vm = new ProfileCustomViewModel
            {
                Facebook = pro.facebook ?? "",
                Twitter = pro.twitter ?? "",
                Instagram = pro.instagram ?? "",
                LinkedIn = pro.linkedin ?? "",
                Handshake = pro.handshake ?? ""
            };
            return vm;
        }
    }
}
