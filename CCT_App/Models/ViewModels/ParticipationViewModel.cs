using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCT_App.Models.ViewModels
{
    public class ParticipationViewModel
    {
        public string ParticipationCode { get; set; }
        public string ParticipationDescription { get; set; }

        public static implicit operator ParticipationViewModel(PART_DEF p)
        {
            ParticipationViewModel vm = new ParticipationViewModel
            {
                ParticipationCode = p.PART_CDE,
                ParticipationDescription = p.PART_DESC
            };

            return vm;
        }
    }
}