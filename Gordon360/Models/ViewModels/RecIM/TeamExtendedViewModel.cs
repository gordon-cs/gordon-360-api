﻿using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM;

public class TeamExtendedViewModel
{
    public int ID { get; set; }
    public ActivityExtendedViewModel Activity { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
    public string? Logo { get; set; }
    public virtual IEnumerable<MatchExtendedViewModel> Match { get; set; }
    public virtual IEnumerable<ParticipantExtendedViewModel> Participant { get; set; }
    public virtual IEnumerable<TeamMatchHistoryViewModel> MatchHistory { get; set; }
    public virtual IEnumerable<TeamRecordViewModel> TeamRecord { get; set; }
    public double SportsmanshipRating { get; set; }
    public string? Affiliation { get; set; }


    public static implicit operator TeamExtendedViewModel(Team t)
    {
        return new TeamExtendedViewModel
        {
            ID = t.ID,
            Activity = t.Activity,
            Name = t.Name,
            Status = t.Status?.Description,
            Logo = t.Logo,
            Affiliation = t.Affiliation,
        };
    }
}