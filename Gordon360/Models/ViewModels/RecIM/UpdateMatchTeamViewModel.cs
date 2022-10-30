using Gordon360.Models.CCT;
using System;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class UpdateMatchTeamViewModel
    {
        public int ID { get; set; }
        public string? Status { get; set; }
        public int? Score { get; set; }
        public int? Sportsmanship { get; set; }
    }
}