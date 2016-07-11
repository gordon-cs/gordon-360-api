using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
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
                GordonID = a.gordon_id.Trim(),
                FirstName = a.firstname.Trim(),
                LastName = a.lastname.Trim(),
                Email = a.email ?? "", // Some random records have null for an email.
                ADUserName = a.AD_Username.Trim() ?? "",
                AccountType = a.account_type.Trim()
            };

            return vm;
        }
    }

    
}