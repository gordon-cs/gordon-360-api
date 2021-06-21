using System;
using System.Collections.Generic;
using System.Linq;

namespace Gordon360.Models.ViewModels
{
    public class ClockInViewModel
    {
        //is true if user has clocked in and false if user is going to clock out
        public bool currentState { get; set; }

        //time of when when user clocked in
        public DateTime timestamp { get; set; }
    }
}
