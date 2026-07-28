using Gordon360.Models.Salesforce;

namespace Gordon360.Tests.Services_Test;

public class SFScheduleServiceTests : ScheduleServiceContractTests
{
    protected override IScheduleService CreateService()
    {
        var mockContext = new Mock<ISalesforceContext>();
        mockContext
            .Setup(context => context.Query<CourseOffering>(It.IsAny<string>()))
            .ReturnsAsync(new SFQueryResult<CourseOffering>
            {
                records =
                [
                    new CourseOffering
                    {
                        Name = "Intro to Testing",
                        LearningCourse = new LearningCourse { SubjectAbbreviation = "CSCI", CourseNumber = "120" },
                        AcademicSession = new AcademicSession
                        {
                            gc_Jenz_Session_Code__c = "202501",
                            gc_Jenz_Subterm_Code__c = "1Q",
                            gc_Jenz_Term_Code__c = "FA",
                            gc_Jenz_Year_Code__c = "2025",
                            AcademicTerm = new AcademicTerm
                            {
                                gc_Jenz_Term_Code__c = "FA",
                                gc_Jenz_Year_Code__c = "2025",
                                Name = "Fall 2025",
                                StartDate = "2025-08-25T00:00:00.000Z",
                                EndDate = "2025-12-12T00:00:00.000Z"
                            }
                        },
                        CourseOfferingParticipants = new SFChildCollection<CourseOfferingParticipant>
                        {
                            records =
                            [
                                new CourseOfferingParticipant
                                {
                                    ParticipantAffiliation = "Teacher",
                                    ParticipantContact = new ParticipantContact { Name = "Test User" }
                                }
                            ]
                        },
                        CourseOfferingSchedules = new SFChildCollection<CourseOfferingSchedule>
                        {
                            records =
                            [
                                new CourseOfferingSchedule
                                {
                                    IsMonday = true,
                                    IsWednesday = true,
                                    StartDate = new DateTime(2025, 8, 25),
                                    EndDate = new DateTime(2025, 12, 12),
                                    StartTime = "09:00:00",
                                    EndTime = "10:00:00",
                                    Location = new Location { ExternalReference = "RNS 101" }
                                }
                            ]
                        }
                    }
                ]
            });

        var userCourses = new SFUserCourses(mockContext.Object);

        var sessionService = new Mock<ISessionService>();
        sessionService
            .Setup(service => service.GetAll())
            .Returns(new[]
            {
                new SessionViewModel
                {
                    SessionCode = "202501",
                    SessionDescription = "Spring 2025",
                    SessionBeginDate = new DateTime(2025, 1, 13),
                    SessionEndDate = new DateTime(2025, 5, 9)
                },
                new SessionViewModel
                {
                    SessionCode = "202509",
                    SessionDescription = "Fall 2025",
                    SessionBeginDate = new DateTime(2025, 8, 25),
                    SessionEndDate = new DateTime(2025, 12, 12)
                }
            });

        var academicTermService = new Mock<IAcademicTermService>();
        academicTermService
            .Setup(service => service.GetAllTermsAsync())
            .ReturnsAsync(new[]
            {
                new YearTermTableViewModel(
                    new YearTermTable
                    {
                        YR_CDE = "2025",
                        TRM_CDE = "FA",
                        YR_TRM_DESC = "Fall 2025",
                        TRM_BEGIN_DTE = new DateTime(2025, 8, 25),
                        TRM_END_DTE = new DateTime(2025, 12, 12),
                        SHOW_ON_WEB = "B"
                    }),
                new YearTermTableViewModel(
                    new YearTermTable
                    {
                        YR_CDE = "2025",
                        TRM_CDE = "SP",
                        YR_TRM_DESC = "Spring 2025",
                        TRM_BEGIN_DTE = new DateTime(2025, 1, 13),
                        TRM_END_DTE = new DateTime(2025, 5, 9),
                        SHOW_ON_WEB = "B"
                    })
            });

        return new ScheduleService(userCourses, sessionService.Object, academicTermService.Object);
    }
}
