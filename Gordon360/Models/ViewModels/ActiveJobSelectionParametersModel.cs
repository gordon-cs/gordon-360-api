using System;
using System.Collections.Generic;
using System.Linq;

namespace Gordon360.Models.ViewModels
{
    public class ActiveJobSelectionParametersModel
    {
        public DateTime SHIFT_START_DATETIME { get; set; }
        public DateTime SHIFT_END_DATETIME { get; set; }
        public int ID_NUM { get; set; }
    }
}