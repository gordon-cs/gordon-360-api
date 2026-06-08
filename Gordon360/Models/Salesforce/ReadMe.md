# Salesforce Integration Guide

### Purpose

This implementation exists to help transition Gordon360 from querying the Jenzabar CCT database directly to retrieving data from Salesforce.

The primary goals are:

* Minimize disruption to the existing codebase.
* Reduce the amount of code that must be rewritten.
* Preserve existing service and controller interfaces whenever possible.
* Centralize Salesforce-specific logic so future changes require modifications in only a few places.
* Take advantage of native C# features such as dependency injection, attributes, reflection, and JSON serialization to reduce repetition and improve maintainability.

This implementation is designed to feel as similar as possible to the existing database-based workflow while using Salesforce as the underlying data source.

---

# Existing Architecture

Before the Salesforce migration, Gordon360 primarily retrieved data from the CCT database.

## Models

Database tables and views are represented by Model classes.

These models are auto-generated using EF Core Power Tools from the CCT database schema.

Example:

```csharp
public partial class DiningInfo
{
    public int StudentId { get; set; }

    [Required]
    [StringLength(8)]
    [Unicode(false)]
    public string SessionCode { get; set; }
}
```

Models represent how data exists in the database.

---

## ViewModels

ViewModels are used to prepare data for the API and UI.

Despite the name, the "View" in ViewModel refers to the application's UI/API view of the data, not database views.

ViewModels often:

* Clean data
* Transform values
* Aggregate information
* Convert database-specific formats into UI-friendly formats

Many ViewModels already contain implicit conversion operators that convert Models into ViewModels.

---

## Context

Database communication is handled through Entity Framework DbContext classes.

For the CCT database, this is:

```csharp
CCTContext
```

The DbContext provides access to tables and views through DbSet properties.

---

## Services

Controllers do not communicate directly with the database.

Instead, controllers use Service classes.

Services are responsible for:

* Querying data
* Filtering data
* Aggregating data
* Transforming data
* Applying business logic

This keeps controllers focused on handling HTTP requests.

---

## Request Flow

The typical request flow is:

```text
HTTP Request
      ?
Controller
      ?
Service
      ?
CCTContext
      ?
Database
```

ASP.NET Core automatically creates controller instances and injects required dependencies through the built-in dependency injection container.

---

# Using Salesforce

The overall architecture remains mostly unchanged.

```text
HTTP Request
      ?
Controller
      ?
Service
      ?
SalesforceContext
      ?
Salesforce API
```


---

## Step 1 - Create a Salesforce Model

Create a model inside:

```text
Models/Salesforce/
```

Naming convention:

```text
SFDiningInfo
SFProfile
SFStudent
```

Example:

```csharp
using Gordon360.Models.Salesforce.Attributes;
using System.Text.Json.Serialization;

[SalesforceObject("Example__c")]
public class SFExample
{
    public static class FieldNames
    {
        public const string ExampleField1 = "ExampleField1__c";
        public const string ExampleField2 = "ExampleField2__c";
    }

    [JsonPropertyName(FieldNames.ExampleField1)]
    public string ExampleField1 { get; set; }

    [JsonPropertyName(FieldNames.ExampleField2)]
    public string ExampleField2 { get; set; }
}
```

### Comments

The `SalesforceObject` attribute allows SalesforceContext to determine which Salesforce Object should be queried.

The `FieldNames` class centralizes Salesforce field names.

The `JsonPropertyName` attributes are used both for:

* JSON deserialization
* Determining which Salesforce fields should be included in generated SOQL queries

Tip: Copy an existing CCT model as a starting point.

---

## Step 2 - Update the ViewModel

ViewModels should continue to represent the data expected by the API and UI.

Many existing ViewModels already contain implicit conversion operators.

Example:

```csharp
public static implicit operator ExampleViewModel(SFExample model)
{
    ...
}
```

This conversion layer is the appropriate place for:

* Data cleanup
* Format conversions
* Type conversions
* Salesforce-specific data normalization

Some ViewModels are instead constructed directly inside Services. You may continue using whichever pattern the existing code already follows.

---

## Step 3 - Create a Salesforce Service

Replace the previous database service with a Salesforce-based implementation.

Example:

```csharp
public class SFExampleService : IExampleService
{
    private readonly SalesforceContext _sfContext;

    public SFExampleService(SalesforceContext sfContext)
    {
        _sfContext = sfContext;
    }

    public async Task<ExampleViewModel> GetExample(...)
    {
        var queryParameter = $@"
            WHERE {SFExample.FieldNames.ExampleField1} = 'Value'
            LIMIT 1";

        var records =
            await _sfContext.Query<SFExample>(queryParameter);

        var record = records.FirstOrDefault();

        return record; 
    }
}
```

### Notes

Salesforce communication is asynchronous.

Methods that previously returned:

```csharp
ExampleViewModel
```

may now need to return:

```csharp
Task<ExampleViewModel>
```

and callers must use `await`.

SOQL documentation:

https://developer.salesforce.com/docs/atlas.en-us.soql_sosl.meta/soql_sosl/sforce_api_calls_soql.htm

Tip: Copy the existing database service as a starting point.

---

## Step 4 - Register the Service

Update:

```text
Services/ServiceExtensions.cs
```

Replace the existing implementation registration with the Salesforce implementation.

Example:

```csharp
services.AddScoped<IExampleService, SFExampleService>();
```

This allows ASP.NET Core dependency injection to automatically provide the Salesforce implementation wherever the interface is requested.


---

## Step 5 - Update Controllers

Controllers usually require minimal changes.

The most common change is making methods asynchronous.

Example:

```csharp
public async Task<ActionResult<ExampleViewModel>> Get()
{
    var result = await _service.GetExample(...);

    return Ok(result);
}
```

---

# Internal Implementation

The Salesforce implementation uses several native C# features to reduce boilerplate.

## Custom Attributes

```csharp
[SalesforceObject("Example__c")]
```

Used to associate a model class with a Salesforce Object.

SalesforceContext reads this attribute through reflection when building queries.

---

## JsonPropertyName Attributes

```csharp
[JsonPropertyName("FirstName__c")]
```

Used by:

* System.Text.Json during deserialization
* SalesforceContext when determining which fields should be included in generated SOQL

---

## Reflection

SalesforceContext uses reflection to:

* Determine Salesforce Object names
* Determine Salesforce field names
* Generate SELECT clauses automatically

This allows most queries to be written without duplicating field definitions.

---

## Dependency Injection

SalesforceContext and Service classes are injected automatically by ASP.NET Core's dependency injection container.

Controllers remain unaware of how the data is actually retrieved.

This makes it possible to swap implementations with minimal changes.

---

# Future Work

The current implementation focuses on retrieving data from Salesforce.

Additional functionality may be added in the future, including:

* Updating Salesforce records
* Creating Salesforce records
* Bulk operations
* Relationship queries
* Additional caching strategies

The design is expected to evolve as new Salesforce use cases are encountered.
