using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class AcademicCheckInViewModel
    {
        public string Holds { get; set; }

        public string personalPhone { get; set; }

        public bool makePrivate { get; set; }

        public bool noPhone { get; set; }

        public int Ethnicity { get; set; }

        public String Race { get; set; }
        public bool FinancialHold { get; set; }
        public bool HighSchoolHold { get; set; }
        public bool MedicalHold { get; set; }
        public bool MajorHold { get; set; }
        public bool RegistrarHold { get; set; }
        public bool LaVidaHold { get; set; }
        public bool MustRegisterForClasses { get; set; }
        public bool NewStudent { get; set; }

        /*
        public static implicit operator AcademicCheckInViewModel(AcademicCheckIn n)
        {
            AcademicCheckInViewModel vm = new AcademicCheckInViewModel
            {
                Race = n.Race,
                Ethnicity = n.Ethnicity,
                Holds = n.Holds,
                phoneNum = n.phoneNum,
                isPrivate = n.isPrivate,
                noPhone = n.noPhone,
            };

            return vm;
        }
        */
    }
}