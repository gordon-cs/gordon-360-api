using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Static.Names;
namespace Gordon360.Models.ViewModels
{
    public class ProfileCustomViewModel
    {
        public string ID { get; set; }
        public string EmailUserName { get; set; }
        public string ImagePath { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string LinkedIn { get; set; }
        public Nullable<bool> show_img { get; set; }

        public static implicit operator ProfileCustomViewModel(PROFILE_IMAGE info)
        {
            ProfileCustomViewModel vm = new ProfileCustomViewModel
            {
                ID = info.id.Trim(),
                EmailUserName = info.username.Trim(),
                ImagePath = info.Img_Path ?? Defaults.DEFAULT_PROFILE_IMAGE_PATH,
                Facebook = info.facebook ?? "",
                Twitter = info.twitter ?? "",
                Instagram = info.instagram ?? "",
                LinkedIn = info.linkedin ?? "",
                show_img = info.show_img ?? true
            };

            return vm;
        }
    }
}