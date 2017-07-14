using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Static.Names;
namespace Gordon360.Models.ViewModels
{
    public class ProfileCustomViewModel
    {
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string LinkedIn { get; set; }
        public string Img_Path { get; set; }
        public string Img_Name { get; set; }
        public string Pref_Img_Path { get; set; }
        public string Pref_Img_Name { get; set; }
    }
}