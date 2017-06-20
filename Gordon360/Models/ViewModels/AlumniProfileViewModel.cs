using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class AlumniProfileViewModel
    {
        public int Row_ID { get; set; }
        public string ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public string EmailUserName { get; set; }
        public string HomeStreet1 { get; set; }
        public string HomeStreet2 { get; set; }
        public string HomeCity { get; set; }
        public string HomeState { get; set; }
        public string HomePostalCode { get; set; }
        public string HomeCountry { get; set; }
        public string HomePhone { get; set; }
        public string ClassYear { get; set; }
        public string Major { get; set; }
        public string ShareName { get; set; }
        public string ShareAddress { get; set; }

        public static implicit operator AlumniProfileViewModel(alumni alu)
        {
            AlumniProfileViewModel vm = new AlumniProfileViewModel
            {
                Row_ID = alu.Row_ID,
                ID = alu.ID.Trim(),
                FirstName = alu.FirstName.Trim(),
                LastName = alu.LastName.Trim(),
                NickName = alu.NickName ?? "", // Just in case some random record has a null user_name 
                EmailUserName = alu.EmailUserName.Trim() ?? "", // Just in case some random record has a null email field
                HomeStreet1 = alu.HomeStreet1 ?? "",
                HomeStreet2 = alu.HomeStreet2 ?? "",
                HomeCity = alu.HomeCity ?? "",
                HomeState = alu.HomeState ?? "",
                HomePostalCode = alu.HomePostalCode ?? "",
                HomeCountry = alu.HomeCountry ?? "",
                HomePhone = alu.HomePhone ?? "",
                ClassYear = alu.ClassYear ?? "",
                Major = alu.Major1 ?? "",
                ShareName = alu.ShareName ?? "",
                ShareAddress =alu.ShareAddress ?? ""
            };

            return vm;
        }
    }
}