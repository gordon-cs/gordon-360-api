using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class ProfileImageViewModel
    {
        public string ProfileID { get; set; }
        public string ProfileUsername { get; set; }
        public string ProfileImagePath { get; set; }
        public string ProfileFacebook { get; set; }
        public string ProfileTwitter { get; set; }
        public string ProfileInstagram { get; set; }
        public string ProfileLinkedIn { get; set; }

        public static implicit operator ProfileImageViewModel(PROFILE_IMAGE info)
        {
            ProfileImageViewModel vm = new ProfileImageViewModel
            {
                ProfileID = info.id.Trim(),
                ProfileUsername = info.username.Trim(),
                ProfileImagePath = info.Img_Path.Trim() ?? "",
                // ProfileFacebook = info.facebook.Trim() ?? "",
                // ProfileTwitter = info.twitter.Trim() ?? "",
                // ProfileInstagram = info.instagram.Trim() ?? "",
                // ProfileLinkedIn = info.linkedin.Trim() ?? ""
            };

            return vm;
        }
    }
}