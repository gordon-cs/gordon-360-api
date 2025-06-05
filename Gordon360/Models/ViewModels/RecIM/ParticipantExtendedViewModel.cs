﻿using Gordon360.Models.CCT;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM;

public sealed record ParticipantExtendedViewModel
{
    public required string Username { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
    public int? GamesAttended { get; set; }
    public string? Status { get; set; }
    public IEnumerable<ParticipantNotificationViewModel>? Notification { get; set; }
    public bool IsAdmin { get; set; }
    public bool AllowEmails { get; set; }
    public string? SpecifiedGender { get; set; }
    public bool IsCustom { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Hall { get; set; }

    public static implicit operator ParticipantExtendedViewModel(ACCOUNT a)
    {
        return new ParticipantExtendedViewModel
        {
            Username = a.AD_Username.Trim() ?? "",
            Email = a.email ?? "",
        };
    }

    public static ParticipantExtendedViewModel? From(ParticipantView p, bool isAdminView = false)
    {
        var result = new ParticipantExtendedViewModel
        {
            Username = p.Username,
            Email = p.Email,
            IsAdmin = p.IsAdmin,
            SpecifiedGender = p.SpecifiedGender,
            AllowEmails = p.AllowEmails,
            IsCustom = p.IsCustom,
            FirstName = p.FirstName,
            LastName = p.LastName,
        };

        // only admin viewers can see a participant's hall
        if (isAdminView == true) result.Hall = p.Hall;

        return result;
    }
}