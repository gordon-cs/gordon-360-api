using Microsoft.Graph.CallRecords;
using System;
using System.Collections.Generic;
using System.Linq;

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
    public CoursesBySessionViewModel(String YearTermCode,IEnumerable<UserCoursesViewModel> courses) : this(
        SessionCode: YearTermCode,
        SessionDescription: UserCoursesViewModel.FormatYearAndTerm(
            courses.FirstOrDefault()?.YR_CDE,
            courses.FirstOrDefault()?.TRM_CDE),
        SessionBeginDate:null,
        SessionEndDate:null,
        AllCourses: courses)
    { }/*
    public CoursesBySessionViewModel(String session, String sessionDescription, DateTime sessionBeginDate, DateTime sessionEndDate, IEnumerable<UserCoursesViewModel> courses) : this(
       SessionCode: session,
       SessionDescription: sessionDescription,
       SessionBeginDate: sessionBeginDate,
       SessionEndDate: sessionEndDate,
       AllCourses: courses)
    { }
    */
}
