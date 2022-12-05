﻿using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class SeriesCreatedViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int ActivityID { get; set; }
        public int TypeID { get; set; }
        public int StatusID { get; set; }

        public static implicit operator SeriesCreatedViewModel(Series s)
        {
            return new SeriesCreatedViewModel
            {
                ID = s.ID,
                Name = s.Name,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                ActivityID = s.ActivityID,
                TypeID = s.TypeID,
                StatusID = s.StatusID
            };
        }
    }

    
}