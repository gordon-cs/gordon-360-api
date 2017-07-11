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
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string LinkedIn { get; set; }
        public string Img_Path { get; set; }
        public string Img_Name { get; set; }
        public string Pref_Img_Path { get; set; }
        public string Pref_Img_Name { get; set; }
        public int preffered_photo { get; set; }
        public int show_pic { get; set; }

        public static implicit operator ProfileCustomViewModel(CUSTOM_PROFILE info)
        {
            ProfileCustomViewModel vm = new ProfileCustomViewModel
            {
            };

            return vm;
        }
    }
}