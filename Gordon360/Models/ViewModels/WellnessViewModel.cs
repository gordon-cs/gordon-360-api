using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class WellnessViewModel
    {

        public Nullable<bool> currentStatus { get; set; } //wether student is feeling symptomatic or not

        public Nullable<bool> userAnswer { get; set; } //answer to wellness question

        public DateTime timestamp { get; set; } //time stamp of when student last answered the question

    }
}