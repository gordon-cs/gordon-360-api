using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class WellnessViewModel
    {

        public Nullable<bool> answerValid { get; set; } //returns true if last answered question is still valid

        public Nullable<bool> userAnswer { get; set; } //answer to wellness question

        public DateTime timestamp { get; set; } //time stamp of when student last answered the question

    }
}