using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class SliderViewModel
    {
        public string FilePath { get; set; }
        public string AltTag { get; set; }
        public string SliderTitle { get; set; }
        public string SliderSubTitle { get; set; }
        public string SliderAction { get; set; }
        public Nullable<int> Width { get; set; }
        public Nullable<int> Height { get; set; }
        public int SortOrder { get; set; }

        public static implicit operator SliderViewModel(C360_SLIDER s)
        {
            SliderViewModel svm = new SliderViewModel
            {
                FilePath = s.strLinkURL + s.strFileName + s.strExtensions,
                AltTag = s.strAltTag,
                SliderTitle = s.strSliderTitle,
                SliderSubTitle = s.strSliderSubTitle,
                SliderAction = s.strSliderAction,
                Width = s.iWidth,
                Height = s.iHeight,
                SortOrder = s.sortOrder
            };

            return svm;
        }

        public string strFileName { get; set; }
        public string strFlashFileName { get; set; }
        public string strAltTag { get; set; }
        public string strLinkURL { get; set; }
        public string strSliderTitle { get; set; }
        public string strSliderSubTitle { get; set; }
        public string strSliderAction { get; set; }
        public Nullable<int> iWidth { get; set; }
        public Nullable<int> iHeight { get; set; }
        public string strExtensions { get; set; }
        public System.DateTime dtModified { get; set; }
        public int sortOrder { get; set; }
    }
}