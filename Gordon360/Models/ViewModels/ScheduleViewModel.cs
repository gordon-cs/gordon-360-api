using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels;

public record ScheduleViewModel(SessionViewModel Session, IEnumerable<ScheduleCourseViewModel> Courses);
