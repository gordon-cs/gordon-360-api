using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class BannerSlidePostViewModel
    {
        public string Title { get; set; }
        public string LinkURL { get; set; }
        public int SortOrder { get; set; }
        public string ImageData { get; set; }
    }
}