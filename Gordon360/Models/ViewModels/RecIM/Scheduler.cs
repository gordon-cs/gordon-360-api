using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class EliminationRound
    {
        public int TeamsInNextRound { get; set; }
        public IEnumerable<MatchViewModel> Match { get; set; }

    }

    public class UploadScheduleRequest
    {
        public int? RoundRobinMatchCapacity { get; set; }
        public int? NumberOfLadderMatches { get; set; }
    }
}