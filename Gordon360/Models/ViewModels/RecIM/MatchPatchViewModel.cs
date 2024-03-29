﻿using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM;

public class MatchPatchViewModel
{
    public DateTime? StartTime { get; set; }
    public int? SurfaceID { get; set; }
    public int? StatusID { get; set; }
    public IEnumerable<int>? TeamIDs { get; set; }
}