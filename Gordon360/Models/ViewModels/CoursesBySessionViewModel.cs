using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels;

public record CoursesBySessionViewModel(
    string SessionCode,
    string SessionDescription,
    DateTime? SessionBeginDate,
    DateTime? SessionEndDate,
    IEnumerable<UserCoursesViewModel> AllCourses)
{
    public CoursesBySessionViewModel(SessionViewModel session, IEnumerable<UserCoursesViewModel> courses) : this(
        SessionCode: session.SessionCode,
        SessionDescription: session.SessionDescription,
        SessionBeginDate: session.SessionBeginDate,
        SessionEndDate: session.SessionEndDate,
        AllCourses: courses)
    { }
}