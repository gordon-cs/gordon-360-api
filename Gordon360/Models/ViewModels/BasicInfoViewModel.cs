using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class BasicInfoViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ADUserName { get; set; }
        public string AccountType { get; set; }
        public string Email { get; set; }

        public static implicit operator BasicInfoViewModel(ACCOUNT a)
        {
            BasicInfoViewModel vm = new BasicInfoViewModel
            {
                FirstName = a.firstname,
                LastName = a.lastname,
                ADUserName = a.AD_Username ?? "",
                AccountType = a.account_type,
                Email = a.email ?? ""
            };

            return vm;
        }
    }

    
}