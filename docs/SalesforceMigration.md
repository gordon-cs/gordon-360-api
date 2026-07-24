# Salesforce Code Migration Guide


## Overview
This document serves as a technical guide for developers migrating the Gordon 360 application's data source from the legacy Jenzabar database to Salesforce. As part of this transition, we are moving away from querying the `CCTContext` directly via Microsoft Entity Framework and adopting a new architecture tailored for the Salesforce API 

 Most of the data in Salesforce is stored similarly to how it was in the Jenzabar Database. A couple differences twe need to be aware of between working with the Jenzabar SQL database and Salesforce. 
 One different is that with SQL we could create views that combined the specific data we needed about something into a View, and the View would behaved the same as a table from the 360's perfective. 
 Another one is that .Net directly supported working with a SQL database through EF Core; so this made it easy to auto generate models of each table in the code. and we could use those to create ModelViews as well. 
 The actual database connection and query was also abstracted. But with Salesforce since we are working with their API we have to handle all of it. 
 
 Our first goal this summer (2026) was to create a system that abstracts all the steps that are resuable in the process. This document explains how to work with this system:


## DTO (Data Transfer Objects)

In the `Models/Salesforce/dto` directory, we create classes that model the objects in Salesforce that we need. Each DTO class has the fields of the Salesforce object we will need: the field names in a DTO class exactly match the Salesforce object field names.

Salesforce allows us to traverse objects through relationship fields in SOQL. We create relationship fields in the DTOs.

* **Parent relationships:** We use an object of the type of the DTO of the parent with the same name that the relationship has in that object. If you set a default value for a parent object, be on the lookout for recursive relationships. These will create the default value infinitely.
* **Child relationships:** We use the collection object `SFChildCollection<T>` where `T` is the DTO of the child objects. The name of the field referencing the collection attribute must be the name that the child relationship has in the Salesforce object.

```csharp
namespace Gordon360.Models.Salesforce;

public class CourseOffering
{
    public string Name { get; set; } = "";

    public LearningCourse LearningCourse { get; set; } = new();
    public AcademicSession AcademicSession { get; set; } = new();
    
    public SFChildCollection<CourseOfferingParticipant> CourseOfferingParticipants { get; set; } = new();
    public SFChildCollection<CourseOfferingSchedule> CourseOfferingSchedules { get; set; } = new();
}
```

*We are almost ready to create the query procedure class. Let’s talk about Salesforce queries first.*

## SOQL (Standard Object Query Language)

Salesforce’s language for querying data is very similar to traditional SQL. SOQL, however, provides several key features that make data queries very efficient.

The Jenzabar database has many data views and procedures to get access to specific information that we show in 360. The logic behind those views and procedures allows us to ignore how the data is structured in Jenzabar.

Creating special joiner objects in Salesforce just for Gordon 360 would add unnecessary structural metadata to Salesforce, but with SOQL we can create queries that span multiple parent-child relationships to obtain the data we need. While this requires a deeper understanding of the structure of the data in Salesforce, it brings more flexibility. 

For example, a student was limited to three majors, since there were 3 fields in the student view representing majors. Now we can create a nested query from a Student Contact to get the student’s Learner Programs and show all of their majors and minors. While this will most likely remain less than or equal to 3, the new data model allows for more.

This example loads a student’s information:

```sql
SELECT 
    Name,
    FirstName,
    MiddleName,
    LastName,
    (
        // this loads the different majors and minors of a student or alum
        SELECT 
            Name,
            LearningProgramPlan.LearningProgram.Type__c,
            LearningProgramPlan.LearningProgram.Name,
            Status
        FROM LearnerPrograms            
    ),
    (
        // this loads the different roles of a person
        SELECT
            Name,
            Description,
            Status,
            RoleType
        FROM Persons 
    ),
    (
        // this will get the student housing information
        SELECT 
            Name,
            Academic_Term__r.Name,
            gc_On_Campus_Location__r.Name,
            gc_On_Campus_Location__r.ParentLocation.Name,
            gc_On_Campus_Location__r.gc_Jenz_Building_Code__c,
            gc_On_Campus_Location__r.Phone,
            gc_On_Campus_Location__r.gc_Jenz_Room_Code__c,
            gc_Status__c
        FROM ContactPointAddresses 
        WHERE AddressType = 'On-Campus'
    ),
    (
        SELECT 
            Name,
            ( 
                // Through this we can get their advisors
                SELECT 
                    Name,
                    PartyRoleRelation.Name,
                    RelatedContact.Name
                FROM CCRContacts 
            )
        FROM Contacts
    )
FROM Account
WHERE RecordType.Name = 'Person Account'
  AND Name = 'Jamie Berry'
```

To create SOQL queries, it is worth exploring the structure of Salesforce Education Cloud and the custom objects that the school currently uses.

> **Note:** It would be great to have a Lucidchart of the newest customizations.

## Procedures

In `Models/Salesforce/Procedures`, we create procedure classes that services use to query the Salesforce data. These replace the old method of querying `CCTContext` directly.

Procedure classes also allow us to query the database much more precisely. Instead of getting all records from a table and filtering them, we can compose queries that yield only the records we want.

Here's an example, which also includes the SOQL query:

```c#
public class SFUserCourses(ISalesforceContext context)
{
    private readonly ISalesforceContext _context = context;

    private const string SoqlTemplate = """
        SELECT
            Name,
            LearningCourse.SubjectAbbreviation,
            LearningCourse.CourseNumber,
            AcademicSession.AcademicTerm.Name,
            AcademicSession.gc_Jenz_Session_Code__c,
            AcademicSession.gc_Jenz_Subterm_Code__c,
            AcademicSession.gc_Jenz_Term_Code__c,
            AcademicSession.gc_Jenz_Year_Code__c,
            (
                SELECT ParticipantAffiliation, ParticipationStatus, ParticipantContact.Name
                FROM CourseOfferingParticipants
                WHERE ParticipantContact.gc_University_Email__c LIKE '{0}%'
            ),
            (
                SELECT Description, IsSunday, IsMonday, IsTuesday, IsWednesday, IsThursday, IsFriday, IsSaturday,
                       Location.ExternalReference, StartDate, EndDate, StartTime, EndTime
                FROM CourseOfferingSchedules
            )
        FROM CourseOffering
        WHERE Id IN (
            SELECT CourseOfferingId
            FROM CourseOfferingParticipant
            WHERE ParticipantContact.gc_University_Email__c LIKE '{0}%'
                AND (NOT (ParticipationStatus='Dropped' OR ParticipationStatus='Withdrew'))
                {1}
        )
    """;

    public async Task<IEnumerable<UserCoursesViewModel>> GetUserCourses(string username, string role = "")
    {
        var roleFilter = string.IsNullOrWhiteSpace(role) ? "" : $"AND ParticipantAffiliation = '{role}'";

        var response = await _context.Query<CourseOffering>(string.Format(SoqlTemplate, username, roleFilter));

        return response?.records?
            .Select(c => MapToViewModel(c, username))
            .ToList() ?? new List<UserCoursesViewModel>();
    }

    private static UserCoursesViewModel MapToViewModel(CourseOffering c, string username)
    {
        var schedule = c.CourseOfferingSchedules.records.FirstOrDefault();
        var participant = c.CourseOfferingParticipants.records.FirstOrDefault();

        return new UserCourses // thi
        {
            Role = participant?.ParticipantAffiliation ?? "",

            YR_CDE = c.AcademicSession.gc_Jenz_Year_Code__c,
            TRM_CDE = c.AcademicSession.gc_Jenz_Term_Code__c,
            SUBTERM_DESC = c.AcademicSession.gc_Jenz_Subterm_Code__c,

            CRS_CDE = $"{c.LearningCourse.SubjectAbbreviation}-{c.LearningCourse.CourseNumber}",
            CRS_TITLE = c.Name,

            BLDG_CDE = schedule?.Location.ExternalReference ?? "",

            MONDAY_CDE = DayCode(schedule?.IsMonday, "M"),
            TUESDAY_CDE = DayCode(schedule?.IsTuesday, "T"),
            WEDNESDAY_CDE = DayCode(schedule?.IsWednesday, "W"),
            THURSDAY_CDE = DayCode(schedule?.IsThursday, "R"),
            FRIDAY_CDE = DayCode(schedule?.IsFriday, "F"),
            SATURDAY_CDE = DayCode(schedule?.IsSaturday, "S"),

            BEGIN_DATE = schedule?.StartDate,
            END_DATE = schedule?.EndDate,

            BEGIN_TIME = ParseTime(schedule?.StartTime),
            END_TIME = ParseTime(schedule?.EndTime)
        };
    }

    private static string DayCode(bool? flag, string code) => flag == true ? code : "";

    private static TimeSpan? ParseTime(string? time)
    {
        if (string.IsNullOrWhiteSpace(time))
        {
            return null;
        }
        else
        {
            var cleanedTime = time.Replace("Z", "");
            var isValid = TimeSpan.TryParse(cleanedTime, out var t);

            return isValid ? t : null;
        }
    }
}
```

## Services

The role of Services classes is essentially unchanged from the Jenzabar era. Those that only depend on other services can remain unchanged, while those that query the database can be changed to use Salesforce procedures.

It is currently preferred to recreate services that need to be changed as classes implementing that service’s interface, calling them `SF<service name>`, and rewriting them to use Salesforce procedures. We then replace the service instantiated in `Services/ServicesExtensions.cs`. This makes it easier to compare the behaviors of the two implementations. If we use an injection to pass the SF Procedure to the service, we need to register it in the `Services/ServicesExtensions.cs` file in the AddSalesforceProcedures method: 
       
```services.AddScoped<SFStudentEmployment>();```



Currently, there are no tests in place for services, as there are no good ways to mock up the Microsoft Entity Framework database model. This makes it difficult to test for differences between implementations. For now, we document expected interface behavior, and fix or document services that rely on undocumented behavior.