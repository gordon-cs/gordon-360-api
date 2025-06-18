using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels;

public record CoursesByTermViewModel(
    string TermCode,
    string TermDescription,
    DateTime? TermBeginDate,
    DateTime? TermEndDate,
    IEnumerable<UserCoursesViewModel> AllCourses)
{
    public CoursesByTermViewModel(YearTermTableViewModel term, IEnumerable<UserCoursesViewModel> courses) : this(
        TermCode: term.TermCode,
        TermDescription: term.Description,
        TermBeginDate: term.BeginDate,
        TermEndDate: term.EndDate,
        AllCourses: courses)
    { }
}
