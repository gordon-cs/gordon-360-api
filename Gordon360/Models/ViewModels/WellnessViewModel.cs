using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class WellnessViewModel
    {

        public Nullable<bool> currentStatus { get; set; } //wether student is feeling symptomatic or not

        public string userAnswer { get; set; } //answer to wellness question

    }
}