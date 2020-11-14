using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class DEPRECATED_WellnessViewModel
    {
        //returns true if last answered question is still valid
        public Nullable<bool> answerValid { get; set; }

        //user's answer to wellness question. either true or false. 
        // True: "I am symptomatic"
        // false: "I am not symptomatic"
        public Nullable<bool> userAnswer { get; set; }

        //time stamp of when user last answered the wellness question
        public DateTime timestamp { get; set; }

    }
}