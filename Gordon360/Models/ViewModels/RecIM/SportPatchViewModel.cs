using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class SportPatchViewModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Rules { get; set; }
        public string? Logo { get; set; }
        public bool IsLogoUpdate { get; set; }

    }
}