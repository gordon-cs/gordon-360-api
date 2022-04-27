using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Models;

namespace Gordon360.Models.ViewModels
{
    public class SliderViewModel
    {
        public string ImagePath { get; set; }
        public string AltTag { get; set; }
        public bool HasCaption { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Action { get; set; }
        public string ActionLink { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public int SortOrder { get; set; }

        public static implicit operator SliderViewModel(Slider_Images s) => new SliderViewModel
        {
            ImagePath = s.Path,
            AltTag = s.Title,
            ActionLink = s.LinkURL,
            Width = s.Width,
            Height = s.Height,
            SortOrder = s.SortOrder
        };
    }
}