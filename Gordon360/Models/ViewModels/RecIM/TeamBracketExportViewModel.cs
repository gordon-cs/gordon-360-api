using Gordon360.Models.CCT;
using System.Collections;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM;

public class TeamBracketExportViewModel
{
    public int id { get; set; }
    public string? resultText { get; set; }
    public bool isWinner { get; set; }
    public string? status { get; set; }
    public string? name { get; set; }
}