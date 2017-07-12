using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Static.Names;
namespace Gordon360.Models.ViewModels
{
    public class PublicProfileCustomViewModel
    {
        public string EmailUserName { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string LinkedIn { get; set; }
        public string office_hours { get; set; }
        public int preffered_photo { get; set; }
        public int show_pic { get; set; }

        public static implicit operator PublicProfileCustomViewModel(ProfileCustomViewModel pro)
        {
            PublicProfileCustomViewModel vm = new PublicProfileCustomViewModel
            {
                EmailUserName = pro.EmailUserName ?? "",
                Facebook = pro.Facebook ?? "",
                Twitter = pro.Twitter ?? "",
                Instagram = pro.Instagram ?? "",
                LinkedIn = pro.LinkedIn ?? "",
                office_hours = pro.office_hours ?? "",
                show_pic = pro.show_pic,
                preffered_photo = pro.preffered_photo
            };
            return vm;
        }
    }
}