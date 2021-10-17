using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class EmailViewModel
    {      
        public string FirstName { get; set; }  
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Description = "";
    }
}