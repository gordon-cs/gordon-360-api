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
        public string Major1 { get; set; }
        public string ShareName { get; set; }
        public string ShareAddress { get; set; }

        public static implicit operator AlumniProfileViewModel(alumni al)
        {
            AlumniProfileViewModel vm = new AlumniProfileViewModel
            {
                Row_ID = al.Row_ID,
                ID = al.ID.Trim(),
                FirstName = al.FirstName.Trim(),
                LastName = al.LastName.Trim(),
                NickName = al.NickName ?? "", // Just in case some random record has a null user_name 
                EmailUserName = al.EmailUserName.Trim() ?? "", // Just in case some random record has a null email field
                HomeStreet1 = al.HomeStreet1 ?? "",
                HomeStreet2 = al.HomeStreet2 ?? "",
                HomeCity = al.HomeCity ?? "",
                HomeState = al.HomeState ?? "",
                HomePostalCode = al.HomePostalCode ?? "",
                HomeCountry = al.HomeCountry ?? "",
                HomePhone = al.HomePhone ?? "",
                ClassYear = al.ClassYear ?? "",
                Major1 = al.Major1 ?? "",
                ShareName = al.ShareName ?? "",
                ShareAddress =al.ShareAddress ?? ""
            };

            return vm;
        }
    }
}