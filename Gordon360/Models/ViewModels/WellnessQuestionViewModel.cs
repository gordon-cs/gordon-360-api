using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class WellnessQuestionViewModel
    {
        public String question { get; set; } //question to the wellness check from back end

        public String yesPrompt { get; set; } //prompt that pops up if answer to question is yes

        public String noPrompt { get; set; } //prompt that pops up if answer to question is no
    }
}