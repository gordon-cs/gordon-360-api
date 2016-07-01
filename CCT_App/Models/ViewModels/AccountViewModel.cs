using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCT_App.Models.ViewModels
{
    public class AccountViewModel
    {
        public string GordonID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ADUserName { get; set; }
        public string AccountType { get; set; }

        public static implicit operator AccountViewModel(ACCOUNT a)
        {
            AccountViewModel vm = new AccountViewModel
            {
                GordonID = a.gordon_id,
                FirstName = a.firstname,
                LastName = a.lastname,
                Email = a.email,
                ADUserName = a.AD_Username,
                AccountType = a.account_type
            };

            return vm;
        }
    }

    
}