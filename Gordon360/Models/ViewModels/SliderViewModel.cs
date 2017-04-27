using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        public Nullable<int> Width { get; set; }
        public Nullable<int> Height { get; set; }
        public int SortOrder { get; set; }

        public static implicit operator SliderViewModel(C360_SLIDER s)
        {
            bool hasCaption = false;
            if (!s.strSliderAction.Equals("") || !s.strSliderTitle.Equals("") || !s.strSliderSubTitle.Equals(""))
            {
                hasCaption = true;
            }
            SliderViewModel svm = new SliderViewModel
            {
                ImagePath = "https://www.gordon.edu" + s.strFileName,
                AltTag = s.strAltTag,
                HasCaption = hasCaption,
                Title = s.strSliderTitle,
                SubTitle = s.strSliderSubTitle,
                Action = s.strSliderAction,
                ActionLink = s.strLinkURL,
                Width = s.iWidth,
                Height = s.iHeight,
                SortOrder = s.sortOrder
            };

            return svm;
        }
    }
}