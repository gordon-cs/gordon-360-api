# Salesforce Code Migration Guide


## Overview
This document serves as a technical guide for developers migrating the Gordon 360 application's data source from the legacy Jenzabar database to Salesforce. As part of this transition, we are moving away from querying the `CCTContext` directly via Microsoft Entity Framework and adopting a new architecture tailored to Salesforce Education Cloud. 

This guide details the new patterns for data access, including creating Data Transfer Objects (DTOs) that mirror Salesforce models, writing efficient SOQL queries for traversing relationships, establishing Procedure classes for targeted data retrieval, and refactoring existing Service classes to implement the new data flow safely.

---

## DTO (Data Transfer Objects)

In the `Models/Salesforce/dto` directory, we create classes that model the objects in Salesforce that we need. Each DTO class has the fields of the Salesforce object we will need: the field names in a DTO class exactly match the Salesforce object field names.

Salesforce allows us to traverse objects through relationship fields in SOQL. We model relationship fields in the DTOs.

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

## SOQL (Standard Object Query Language)

Salesforce’s language for querying data is very similar to traditional SQL. SOQL, however, makes it even easier to traverse complex relationships within a query.

The Jenzabar database has many data views and procedures to get access to specific information that we show in 360. The logic behind those views and procedures allows us to ignore how the data is structured in Jenzabar.

Creating special joiner objects in Salesforce just for Gordon 360 would add unnecessary structural metadata to Salesforce, but with SOQL we can create queries that span multiple parent-child relationships to obtain the data we need. While this requires a deeper understanding of the structure of the data in Salesforce, it brings more flexibility. 

For example, a student was limited to three majors, since the student view had three fields representing majors. Now we can create a nested query from a Student Contact to get the student’s Learner Programs and show an unlimited number of their majors and minors. While it is rare for students to have more than three majors, the new data model gives us greater flexibility.

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

To create SOQL queries, it is necessary to know the structure of Salesforce Education Cloud and the custom objects that the school currently uses.

> **Note:** It would be great to have a Lucidchart diagram of the newest customizations. A lucidchart diagram of the base Salesforce Education Cloud model can be found [here](https://lucid.app/lucidchart/30cc34ce-f426-43f9-aae2-22f642e24961/edit?invitationId=inv_6b0a377b-f004-4415-82a9-c167c8245d2a&page=0_0#)

## Procedures

In `Models/Salesforce/Procedures`, we create procedure classes that services use to query the Salesforce data. These replace the old method of querying `CCTContext` directly.

Procedure classes also allow us to query the database much more precisely. Instead of getting all records from a table and filtering them, we can compose smaller queries that yield only the records we want.

## Services

The role of Services classes is essentially unchanged from the Jenzabar era. Those that only depend on other services can remain unchanged, while those that query the database can be changed to use Salesforce procedures.

It is currently preferred to recreate services that need to be changed as classes implementing that service’s interface, calling them `SF<service name>`, and rewriting them to use Salesforce procedures. We then replace the service instantiated in `Services/ServicesExtensions.cs`. This makes it easier to compare the behaviors of the two implementations.

Currently, there are no tests in place for services, as there are no good ways to mock up the Microsoft Entity Framework database model. This makes it difficult to test for differences between implementations. For now, we document expected interface behavior, and fix or document services that rely on undocumented behavior.