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
        public string Barcode { get; set; }
        public int show_pic { get; set; }
        public int ReadOnly { get; set; }

        public static implicit operator AccountViewModel(ACCOUNT a)
        {
            AccountViewModel vm = new AccountViewModel
            {
                GordonID = a.gordon_id.Trim(),
                FirstName = a.firstname.Trim(),
                LastName = a.lastname.Trim(),
                Email = a.email ?? "", // Some random records have null for an email.
                ADUserName = a.AD_Username.Trim() ?? "",
                AccountType = a.account_type.Trim(),
                Barcode = a.barcode ?? "",
                show_pic = a.show_pic,
                ReadOnly = a.ReadOnly
            };

            return vm;
        }
    }

    
}