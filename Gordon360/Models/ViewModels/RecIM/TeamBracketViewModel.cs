using Gordon360.Models.CCT;
using System.Collections;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM;
// API Internal calculation viewmodel
public class TeamBracketExtendedViewModel
{
    public int TeamID { get; set; }
    public string? Score { get; set; }
    public bool IsWinner { get; set; }
    public string? Status { get; set; }
    public string? TeamName { get; set; }
}