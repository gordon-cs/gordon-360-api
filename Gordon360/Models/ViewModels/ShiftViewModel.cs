﻿namespace Gordon360.Models.ViewModels
{
    public class ShiftViewModel
    {
        public int ID { get; set; }
        public int EML { get; set; }
        public System.DateTime SHIFT_START_DATETIME { get; set; }
        public System.DateTime SHIFT_END_DATETIME { get; set; }
        public string HOURS_WORKED { get; set; }
        public string SHIFT_NOTES { get; set; }
        public string LAST_CHANGED_BY { get; set; }
    }
}