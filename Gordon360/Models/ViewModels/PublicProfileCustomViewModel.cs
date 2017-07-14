using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Static.Names;
namespace Gordon360.Models.ViewModels
{
    public class PublicProfileCustomViewModel
    {
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string LinkedIn { get; set; }

        public static implicit operator PublicProfileCustomViewModel(ProfileCustomViewModel pro)
        {
            PublicProfileCustomViewModel vm = new PublicProfileCustomViewModel
            {
                Facebook = pro.Facebook ?? "",
                Twitter = pro.Twitter ?? "",
                Instagram = pro.Instagram ?? "",
                LinkedIn = pro.LinkedIn ?? "",
            };
            return vm;
        }
    }
}