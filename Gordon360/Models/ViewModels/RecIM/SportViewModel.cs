using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM;

public class SportViewModel
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Rules { get; set; }
    public string Logo { get; set; }

    public static implicit operator SportViewModel(Sport s)
    {
        return new SportViewModel
        {
            ID = s.ID,
            Name = s.Name,
            Description = s.Description,
            Rules = s.Rules,
            Logo = s.Logo,
        };
    }

}