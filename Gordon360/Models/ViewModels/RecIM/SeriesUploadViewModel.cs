using Gordon360.Models.CCT;
using System;

namespace Gordon360.Models.ViewModels.RecIM;

public class SeriesUploadViewModel
{
    public string Name { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int ActivityID { get; set; }
    public int TypeID { get; set; }
    public int? NumberOfTeamsAdmitted { get; set; }
    public int? ScheduleID { get; set; }
    public int? WinnerID { get; set; }
    public int? Points { get; set; }
    public Series ToSeries(int activityInheritiedSeriesScheduleID = 0)
    {
        return new Series
        {
            Name = this.Name,
            StartDate = this.StartDate,
            EndDate = this.EndDate,
            ActivityID = this.ActivityID,
            TypeID = this.TypeID,
            StatusID = 2, //default in-progress series
            ScheduleID = this.ScheduleID ?? activityInheritiedSeriesScheduleID, //updated when admin is ready to set up the schedule
            WinnerID = this.WinnerID,
            Points = this.Points ?? 0
        };
    }
}