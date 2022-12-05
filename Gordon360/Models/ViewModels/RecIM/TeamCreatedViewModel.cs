using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class TeamCreatedViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int StatusID { get; set; }
        public int ActivityID { get; set; }
        public string? Logo { get; set; }
        public static implicit operator TeamCreatedViewModel(Team t)
        {
            return new TeamCreatedViewModel
            {
                ID = t.ID, 
                Name = t.Name, 
                StatusID = t.StatusID,  
                ActivityID = t.ActivityID,
                Logo = t.Logo
            };
        }
    }
}