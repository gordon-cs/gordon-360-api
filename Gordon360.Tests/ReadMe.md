# Gordon360 Controller Unit Test Guide 🧪🚀

**Tools used:** [xUnit](https://xunit.net/) & [Moq](https://github.com/moq/moq4)

This guide will walk you through how to write effective and maintainable unit tests for controller logic in the Gordon360 backend. It focuses on testing with mocked services, simulating authentication, and understanding xUnit and Moq syntax—all aimed at improving test coverage and reducing manual errors during API development.

---

## 📂 Table of Contents 📘

1. [Setting Up Test Environment](#1-setting-up-test-environment-️)
2. [Moq Setup](#2-moq-setup-)
3. [xUnit and Test Structure Format](#3-xunit-and-test-structure-format-)
4. [Authentication Setup for Test Purposes](#4-authentication-setup-for-test-purposes-)
5. [Actual Test Demo](#5-actual-test-demo-)
6. [ChatGPT Prompt Tips for Writing Tests](#6-chatgpt-prompt-tips-for-writing-tests-)
7. [Test Dubugging Tips](#7-test-dubugging-tips-)
8. [Automating Tests with GitHub Actions and Git Hooks](#8-automating-tests-with-github-actions-and-git-hooks-)

---

## 1. Setting Up Test Environment 🛠️  

To set up the test project in **Gordon360** from scratch:
Remeber that Step1 to Step3 are only needed if you haven't set up the test project yet. If you already have a test project, skip to Step 4 to start.

### Step 1: Create the Test Project

If not already created, create a new test project:

```bash
dotnet new xunit -n Gordon360.Tests
```

### Step 2: Reference Main Project

Ensure the test project references the main Gordon360 project:

```bash
cd Gordon360.Tests
dotnet add reference ../Gordon360/Gordon360.csproj
```

### Step 3: Install Required NuGet Packages

Install the following packages:

```bash
dotnet add package Moq

dotnet add package Microsoft.AspNetCore.Mvc

dotnet add package Microsoft.AspNetCore.Http
```

### Step 4: Folder Structure
Below we show how to create a test for the Profiles Controller.  The files associated with the tests need to be organized in a folder hierarchy under `Gordon360.Tests`.  In the case of Profiles Controller tests the hierarchy should look like

```
Gordon360.Tests/
├── Controllers_Test/   # create this folder if it does not exist
│   └── ProfilesControllerTests.cs # or any other controller you are testing
```

### Step 5: Import Namespaces for Test Files

Ensure your the namespace needed in your test file are included in "TestUsings.cs":

#### ✅ Global using directives make these namespaces available across all test files
#### This reduces redundancy and keeps test files clean and focused

```
global using Xunit;
global using Moq;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Http;
global using System.Security.Claims;
global using Gordon360.Controllers;
global using Gordon360.Services;
global using Gordon360.Models.ViewModels;
```

### Step 6: Run Tests
go to your test project folder (Gordon360.Tests):

```bash
cd Gordon360.Tests
```

run your tests in terminal using:

```bash
dotnet test
```



---

## 2. Moq Setup 🎭

Moq is used to simulate dependencies so you can test controller logic in isolation.

### Explaining `<IProfileService>`

The generic parameter `IProfileService` in `new Mock<IProfileService>()` refers to the interface that defines the service logic your controller depends on. In Gordon360, interfaces like `IProfileService`, `ISessionService`, and `IScheduleService` define operations such as fetching student profiles or course schedules.

You mock the interface so that you can control what data it returns during tests — without relying on the real database or internal service logic. Replace `IProfileService` with the actual service interface your controller uses.

Example:

```csharp
var mockProfileService = new Mock<IProfileService>();
var mockScheduleService = new Mock<IScheduleService>();
```

### Naming Your Mock Object

* Use descriptive names like `mockProfileService` or `mockScheduleService`
* Prefix with `mock` to make intent clear

```csharp
var mockProfileService = new Mock<IProfileService>();
```

### Setting Up a Method

Moq is used so you can **control what a method returns** without executing real service logic or hitting the database. This keeps tests **fast, isolated, and predictable**.

* `Setup()` defines what should happen when a method is called
* `Returns()` defines what value to return in response

```csharp
mockProfileService.Setup(service => service.GetStudentProfile("targetuser"))
                 .Returns(expectedProfile);
```

This example means: *"When ******`GetStudentProfile("targetuser")`****** is called, return ******`expectedProfile`****** instead of executing actual logic."*

### Note:

When mocking a return value, make sure the object you return is the **exact ViewModel type** expected by the controller (e.g., `StudentProfileViewModel`, `PublicStudentProfileViewModel`).
If you pass the wrong type, the test may fail with casting or serialization errors.

### Demo: Defining a Complete Expected Output & ViewModel Construction

When using `.Returns(expectedResult)` in Moq, you **must pass in a fully constructed object** with all required fields populated, especially if the controller expects to access them.

```csharp
var expected = new StudentProfileViewModel
{
    ID = "1",
    FirstName = "Test",
    LastName = "Student",
    Title = "Mr.",
    MiddleName = "M",
    Suffix = "Jr",
    NickName = "Tester",
    OnOffCampus = "On",
    OnCampusBuilding = "Chase",
    OnCampusRoom = "101",
    OnCampusPhone = "555-1111",
    OnCampusPrivatePhone = "555-1112",
    OnCampusFax = "555-1113",
    OffCampusStreet1 = "123 Main St",
    OffCampusStreet2 = "",
    OffCampusCity = "Wenham",
    OffCampusState = "MA",
    OffCampusPostalCode = "01984",
    OffCampusCountry = "USA",
    OffCampusPhone = "",
    OffCampusFax = "",
    HomeStreet1 = "456 Home St",
    HomeStreet2 = "",
    HomeCity = "Wenham",
    HomeState = "MA",
    HomePostalCode = "01984",
    HomeCountry = "USA",
    HomePhone = "555-2222",
    HomeFax = ""
};

mockProfileService.Setup(s => s.GetStudentProfile("targetuser"))
                  .Returns(expected);
```

➡️ **Important**: If you skip required fields and the controller tries to access them, the test will throw an error.

#### ViewModel Construction: Input Requirements

Some view models in this project are designed to be constructed from another view model or an entity model (database object), rather than through property initializers or a parameterless constructor.  
**You must ensure that the input you provide matches the expected type and format required by the view model's constructor.**

**General Rule:**  
If a view model's constructor requires another model (entity or view model), you must create and fully populate that input object first, then pass it to the view model's constructor:

```csharp
var input = new SomeEntityOrViewModel { /* set properties */ };
var viewModel = new SomeViewModel(input);
```

**You cannot:**
- Use property initializers (e.g., `new SomeViewModel { ... }`) unless the view model supports it.
- Use a parameterless constructor if the view model does not provide one.

**Why?**  
This ensures that all required data and mapping logic are handled in one place, and prevents runtime errors or unexpected behavior.

**Tip:**  
Always check the constructor signature of a view model before using it. If it requires another model as input, set up that input object with all necessary properties.

##### Complete Example 1: Entity to ViewModel (YearTermTableViewModel)

```csharp
using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;
using System;

// Step 1: Create the YearTermTable entity with all required properties
var yearTermEntity = new YearTermTable
{
    YR_CDE = "2024",
    TRM_CDE = "SP",
    TRM_BEGIN_DTE = new DateTime(2024, 1, 10),
    TRM_END_DTE = new DateTime(2024, 5, 10),
    YR_TRM_DESC = "Spring 2024",
    PRT_INPROG_ON_TRAN = "Y",
    CENSUS_PERCENTAGE = 100,
    SHOW_ON_WEB = "Y",
    ONLINE_ADM_APP_OPEN = "Y",
    PREREG_STS = "Y",
    APPROWVERSION = new byte[] { 1 },
    // Optional: Set other properties as needed
};

// Step 2: Pass the entity to the YearTermTableViewModel constructor
var viewModel = new YearTermTableViewModel(yearTermEntity);

// Now you can use viewModel in your test or application logic
Console.WriteLine(viewModel.Description); // Output: Spring 2024
```

### Using Parameters

* Use `It.IsAny<T>()` when you want to allow any argument of a specific type:

```csharp
mockService.Setup(s =>
    s.GetProfile(It.IsAny<string>())
).Returns(expected);
```

* Use `It.Is<T>(predicate)` to define more specific conditions:

```csharp
mockService.Setup(s =>
    s.GetProfile(It.Is<string>(id => id.StartsWith("stu_")))
).Returns(expected);
```

This allows the mock to respond differently depending on the input value.

[Learn more about Moq](https://github.com/moq/moq4)


## 3. xUnit and Test Structure Format 🧱

### Test Structure Format

Organize your xUnit test classes following a clear format:

#### Namespace and Class Naming

* Namespace should follow project folder structure: `Gordon360.Tests.Controllers_Test`
* Test class name should match the controller under test, suffixed with `Tests`

```csharp
namespace Gordon360.Tests.Controllers_Test
{
    public class ProfilesControllerTests
    {
        // test methods here
    }
}
```

#### Test Method Pattern (AAA)

Each test should follow this pattern:

```csharp
[Fact]
public void MethodName_Condition_ExpectedBehavior()
{
    // Arrange: set up test inputs and mocks

    // Act: call the method under test

    // Assert: validate the result
}
```

### Common Assert Examples

* `Assert.IsType<T>(object)` — checks that the returned object is of the expected type:

  ```csharp
  Assert.IsType<OkObjectResult>(result);
  ```

* `Assert.Equal(expected, actual)` — verifies values match:

  ```csharp
  Assert.Equal("Test", student.FirstName);
  ```

* `Assert.NotNull(object)` — ensures the result is not null:

  ```csharp
  Assert.NotNull(result);
  ```

### Verifying Method Calls with Moq

Use `Verify()` to confirm that a mocked method was called:

```csharp
mockService.Verify(s => s.GetStudentProfile("targetuser"), Times.Once);
```

* `Times.Once` ensures the method was called exactly once.
* Other options include `Times.Never`, `Times.AtLeastOnce`, etc.

[Learn more about xUnit](https://xunit.net/)

---

## 4. Authentication Setup for Test Purposes 🔐

In Gordon360, access control is handled by a static `AuthUtils` class that reads user identity from `HttpContext.User`. Because static classes can't be easily mocked, we simulate authenticated users in unit tests by manually injecting claims into a `ControllerContext`.

### Why This Matters

Because you can't mock static methods (like `AuthUtils.GetGroups()`), you must inject the expected identity and roles manually to test how the controller behaves based on the current user's authorization level.

### Where to Find Available Roles (Groups)

The valid role claims for Gordon360 (e.g., `360-Student-SG`, `360-Faculty-SG`, `360-SiteAdmin`) are defined in:

```
Gordon360/Enums/AuthGroup.cs
```

Use these values to simulate user roles accurately in your test identity setup.

### Step 1: Create Identity

Use `ClaimsIdentity` to simulate a user's identity with specific claims:

```csharp
var identity = new ClaimsIdentity(new[]
{
    new Claim(ClaimTypes.Name, "viewer"),
    new Claim("groups", "360-Student-SG")
}, "mock");
```

This defines a user named "viewer" who belongs to the `360-Student-SG` group.

### Step 2: Insert Identity into Controller

Manually assign the identity to the controller's context:

```csharp
controller.ControllerContext = new ControllerContext
{
    HttpContext = new DefaultHttpContext
    {
        User = new ClaimsPrincipal(identity)
    }
};
```

This setup mimics an authenticated request for controller testing.

---

## 5. Actual Test Demo 🧪

```csharp
[Fact]
public void GetUserProfile_AsStudent_ReturnsExpectedProfile()
{
    // Arrange section: This is where we prepare everything needed for the test.
    var mockService = new Mock<IProfileService>();

    var expected = new StudentProfileViewModel
    {
        ID = "1",
        FirstName = "Test",
        LastName = "Student",
        Title = "Mr.",
        MiddleName = "M",
        Suffix = "Jr",
        NickName = "Tester",
        OnOffCampus = "On",
        OnCampusBuilding = "Chase",
        OnCampusRoom = "101",
        OnCampusPhone = "555-1111",
        OnCampusPrivatePhone = "555-1112",
        OnCampusFax = "555-1113",
        OffCampusStreet1 = "123 Main St",
        OffCampusStreet2 = "",
        OffCampusCity = "Wenham",
        OffCampusState = "MA",
        OffCampusPostalCode = "01984",
        OffCampusCountry = "USA",
        OffCampusPhone = "",
        OffCampusFax = "",
        HomeStreet1 = "456 Home St",
        HomeStreet2 = "",
        HomeCity = "Wenham",
        HomeState = "MA",
        HomePostalCode = "01984",
        HomeCountry = "USA",
        HomePhone = "555-2222",
        HomeFax = ""
    };

    mockService.Setup(s => s.GetStudentProfile("targetuser"))
               .Returns(expected);

    var controller = new ProfilesController(mockService.Object, null, null, null);

    var identity = new ClaimsIdentity(new[]
    {
        new Claim(ClaimTypes.Name, "viewer"),
        new Claim("groups", "360-Student-SG")
    }, "mock");

    controller.ControllerContext = new ControllerContext
    {
        HttpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(identity)
        }
    };

    // Act
    var result = controller.GetProfile("targetuser");
    var okResult = result as OkObjectResult;

    // Assert
    Assert.NotNull(okResult);
    Assert.Equal(expected, okResult.Value);
}
```

## Helper Function Demo: Reusable Setup Code 🧰

To keep tests clean and reduce repetition, define helper methods to reuse across different test cases.

### ✅ ViewModel Helper Example

Instead of manually writing the full `StudentProfileViewModel` every time, move it into a static helper:

```csharp
public static class TestHelpers
{
    public static StudentProfileViewModel CreateStudentProfileViewModel()
    {
        return new StudentProfileViewModel
        {
            ID = "1",
            FirstName = "Test",
            LastName = "Student",
            Title = "Mr.",
            MiddleName = "M",
            Suffix = "Jr",
            NickName = "Tester",
            OnOffCampus = "On",
            OnCampusBuilding = "Chase",
            OnCampusRoom = "101",
            OnCampusPhone = "555-1111",
            OnCampusPrivatePhone = "555-1112",
            OnCampusFax = "555-1113",
            OffCampusStreet1 = "123 Main St",
            OffCampusStreet2 = "",
            OffCampusCity = "Wenham",
            OffCampusState = "MA",
            OffCampusPostalCode = "01984",
            OffCampusCountry = "USA",
            OffCampusPhone = "",
            OffCampusFax = "",
            HomeStreet1 = "456 Home St",
            HomeStreet2 = "",
            HomeCity = "Wenham",
            HomeState = "MA",
            HomePostalCode = "01984",
            HomeCountry = "USA",
            HomePhone = "555-2222",
            HomeFax = ""
        };
    }
}
```

### ✅ Authentication Setup Helper Example

Encapsulate the logic for injecting a user with a specific group claim into the controller:

```csharp
public static class AuthTestHelper
{
    public static void InjectAuthenticatedUser(Controller controller, string username, string group)
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim("groups", group)
        }, "mock");

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(identity)
            }
        };
    }
}
```

### ✅ How to Use in Test File (Full Demo)

This example test class shows how to use both helper functions in a real-world test scenario for the `ProfilesController`.

```csharp
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Gordon360.Controllers;
using Gordon360.Services;
using Gordon360.Models.ViewModels;

namespace Gordon360.Tests.Controllers_Test
{
    public class ProfilesControllerTests
    {
        [Fact]
        public void GetUserProfile_AsStudent_ReturnsExpectedProfile()
        {
            // Arrange
            var mockService = new Mock<IProfileService>();

            // ✅ Use TestHelpers to create expected profile ViewModel
            var expected = TestHelpers.CreateStudentProfileViewModel();
            mockService.Setup(s => s.GetStudentProfile("targetuser")).Returns(expected);

            var controller = new ProfilesController(mockService.Object, null, null, null);

            // ✅ Use AuthTestHelper to inject mock authenticated user
            AuthTestHelper.InjectAuthenticatedUser(controller, "viewer", "360-Student-SG");

            // Act
            var result = controller.GetProfile("targetuser");
            var okResult = result as OkObjectResult;

            // Assert
            Assert.NotNull(okResult);
            Assert.Equal(expected, okResult.Value);
        }
    }
}
```




---

## 6. ChatGPT Prompt Tips for Writing Tests 💬🤖

### Process for Using ChatGPT to Write a Test

1. **Enter the prompt**:

   > "Write a unit test to test the logic for the method below (method in the controller) using Moq and xUnit, and set up authentication manually based on (provide the group you want to test)"

2. **Check the test that ChatGPT generates** to ensure it follows the guidelines described in this document (e.g., Moq setup, full expected output, proper authentication setup).

3. **If the expected output is not fully defined**, you can prompt:

   > "Update the test file to create expected output with full parameters based on (provide ViewModel such as StudentProfileViewModel)"

   ⚠ **Note**: ChatGPT is sometimes reluctant to fill in the entire ViewModel because it's long. It often creates only a few parameters and assumes it will work — which usually causes the test to fail or be incomplete.

4. **To handle this yourself**, you can ask:

   > "Create a test expected output (dummy data) based on ViewModel (give full ViewModel name)"

---

## 7. Test Dubugging Tips 🐞

Carefully read the guidelines in this document if you want to avoid 90% of the frustration in writing, running, and fixing your controller unit tests.

Happy testing!

---

## 8. Automating Tests with GitHub Actions and Git Hooks 🔄🪝

To ensure your controller tests run **automatically**, you can use two approaches:

- ☁️ **GitHub Actions**: runs tests remotely every time code is pushed to GitHub.
- 💻 **Git Hooks**: runs tests locally before each commit.

Both are important for enforcing test reliability and preventing broken code from being committed or deployed.

---

### ☁️ Option 1: Run Tests on Push with GitHub Actions

#### 🔧 Setup Instructions

1. **Find or Create the Workflow File**

Check if the file `.github/workflows/ci-cd.yml` already exists in your project. If not, create the following folder and file structure:

```
.github/workflows/ci-cd.yml
```

2. **Add the Following YAML Configuration** (or just add the test step if the file already exists)

```yaml
name: Continuous Integration & Deployment

on: push

jobs:
  build:
    runs-on: windows-latest

    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET SDK
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.x'
          cache: true
          cache-dependency-path: Gordon360/packages.lock.json

      - name: Install dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build

      # ✅ Add this test step:
      - name: Run Tests
        run: dotnet test --no-build --verbosity normal
```

> ✅ This ensures all tests must pass before any deployment job continues.

3. **Push to GitHub**

After committing and pushing this file, GitHub will automatically run your tests with each new push. You can monitor progress in the **Actions tab** of your repository.

---

### 💻 Option 2: Run Tests Before Commit with Git Hook

You can add a **pre-commit Git hook** that runs your unit tests before any commit is finalized.

#### 🛠 Setup Pre-commit Hook (Manual — Recommended)


1️⃣ Navigate to your repo root
```bash

cd path/to/your/repo

Example (on Windows Virtual Machine):

C:\Users\Your__UserName\Source\Repos\gordon-cs\gordon-360-api
```

2️⃣ Open .git/hooks/pre-commit 

    If the file does not exist, create a new file named pre-commit in .git/hooks/.



3️⃣ Copy and paste the following script into pre-commit file:
```bash

#!/bin/sh
echo "Running build before commit..."

dotnet build --no-restore --verbosity quiet
BUILD_EXIT_CODE=$?

if [ $BUILD_EXIT_CODE -ne 0 ]; then
  echo " Build failed. Commit aborted."
  exit 1
fi

echo "Running tests before commit..."

dotnet test gordon360.Tests/gordon360.Tests.csproj --no-build --verbosity quiet
TEST_EXIT_CODE=$?

if [ $TEST_EXIT_CODE -ne 0 ]; then
  echo " Tests failed. Commit aborted."
  exit 1
else
  echo " Build and tests passed. Proceeding with commit."
fi

```

> ⚠️ This only runs locally on your machine. Others won't get it unless shared via tools like Husky.

---

