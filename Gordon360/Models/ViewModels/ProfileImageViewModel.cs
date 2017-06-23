using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class ProfileImageViewModel
    {
        public string ID { get; set; }
        public string EmailUserName { get; set; }
        public string ImagePath { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string LinkedIn { get; set; }

        public static implicit operator ProfileImageViewModel(PROFILE_IMAGE info)
        {
            ProfileImageViewModel vm = new ProfileImageViewModel
            {
                ID = info.id.Trim(),
                EmailUserName = info.username.Trim(),
                ImagePath = info.Img_Path ?? "",
                Facebook = info.facebook ?? "",
                Twitter = info.twitter ?? "",
                Instagram = info.instagram ?? "",
                LinkedIn = info.linkedin ?? ""
            };

            return vm;
        }
    }
}