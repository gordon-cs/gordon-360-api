using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class AcademicCheckInViewModel
    {
        public bool isCheckedIn { get; set; }
        public Nullable<string> Race { get; set; }

        public Nullable<int> Ethnicity { get; set; }

        public string Holds { get; set; }

        public string phoneNum { get; set; }

        public bool isPrivate { get; set; }

        public Nullable<bool> noPhone { get; set; }
        public static implicit operator AcademicCheckInViewModel(CheckInData n)
        {
            AcademicCheckkInViewModel vm = new AcademicCheckInViewModel
            {
                isCheckedIn = n.isCheckedIn,
                Race = n.Race,
                Ethnicity = n.Ethnicity,
                Holds = n.Holds,
                phoneNum = n.phoneNum,
                isPrivate = n.isPrivate,
                noPhone = n.noPhone,
            };

            return vm;
        }
    }
}