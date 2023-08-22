﻿using Gordon360.Models.CCT;
namespace Gordon360.Models.ViewModels;

public class ProfileCustomViewModel
{
    public string Facebook { get; set; }
    public string Twitter { get; set; }
    public string Instagram { get; set; }
    public string LinkedIn { get; set; }
    public string Handshake { get; set; }
    public string PlannedGradYear { get; set; }
    public string Calendar { get; set; }

    public static implicit operator ProfileCustomViewModel?(CUSTOM_PROFILE? pro)
    {
        if (pro == null)
        {
            return null;
        }

        return new ProfileCustomViewModel
        {
            Facebook = pro.facebook ?? "",
            Twitter = pro.twitter ?? "",
            Instagram = pro.instagram ?? "",
            LinkedIn = pro.linkedin ?? "",
            Handshake = pro.handshake ?? "",
            PlannedGradYear = pro.PlannedGradYear ?? "",
            Calendar = pro.calendar ?? ""
        };
    }
}
