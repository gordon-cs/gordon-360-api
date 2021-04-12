# Apartment Application API

## Intention and Use 

See the [design documentation]() on the frontend for the intention and use of the feature as a whole. 

## Design Overview

As a typical feature of the Gordon 360 API, the apartment application API is written in C#, using HTTP to communicate with the UI and storing and managing data in the SQL database CCT. Its job is to make a concise set of methods available to the UI so that the UI can add, remove, and access exactly what it needs to/from the database.

The code that is unique to the apartment application API has just five parts: 
- the Housing service interface
- the Housing service methods
- the view models
- the auto-generated descriptions of the apartment application data tables in the CCT edmx
- and the Housing controller methods

However, there is much code in the Gordon 360 API that these five parts depend on. Some of it is obvious because all feature APIs depend on it. The rest of it is code that only some feature APIs use, so it is important to note that the apartment application API should be counted among this subset of APIs.

![image](https://user-images.githubusercontent.com/40189910/109569411-69a52800-7ab6-11eb-9949-e5486ff3fa04.png)

The interaction of these five parts is represented in the above diagram and can be described as follows: The _service interface_ is a declaration of methods that each perform a transaction in the database (a series of database operations that go hand-in-hand). The _Housing service methods_ implement these transactions, using the _view models_ to hold different combinations of data and calling stored procedures defined in the database. The _edmx descriptions_ give the API an understanding of the apartment application tables so that it can apply stored procedures to them. Lastly, the _Housing controller methods_ define the interface the UI has with the Housing service methods. They are the endpoints the UI calls to access the API. This is also where some of the code that is not unique to the apartment application API comes in, as the Housing controller methods call service methods of other features. Thus, the advantage to having a controller separate from a service is that functionality from other features can be used without it defining the basic building blocks of the feature.



## Design Details 

### Housing Controller Methods / API Routes

Location: `Gordon360/ApiControllers/HousingController.cs`

Route prefix: `/api/housing/apartment/`

These are the endpoints that the UI calls to interact with the API.
The Housing Controller methods have the task of calling the appropriate apartment application service method, a method called by a similar name, and feeding it the input from the UI. They also often have to use methods of other services to acquire additional input that must also be fed to the appropriate service method. Sometimes, the input a controller method receives must be parsed so that pieces of it can be converted into a format the service method accepts. Even if no formatting or extra data is required, the controller is still responsible for validating that the input is acceptable. 

### Housing Service Methods

Location: `Gordon360/Services/HousingService.cs`

The Housing service methods each use the database to perform a transaction for a specific use case. They call one or more stored procedures defined in the CCT database to get, insert, and/or update the data in the apartment application tables. The same service method that retrieves data may also prepare the retrieved data so that it fits a desired format, or it may even derive values from the data retrieved. Service methods that add or update data may also have to filter the input they receive to determine what of it must actually be entered into the tables. Thus, they are not often reusable.

Note that it is intended that any other 360 features related to housing at Gordon College have their services and controllers implemented in `HousingService.cs` and `HousingController.cs`.

### Other Services Used

Other services the apartment application API uses that are not defined in HousingService.cs should be declared underneath the declaration of the HousingController’s private instance of the HousingService. In other words, you can find their instance declaration underneath `private IHousingService _housingService;`

### View Models

Location: `Gordon360/Models/ViewModels/`

View models are simple public classes that consist only of a set of public variables. They have three different uses in the apartment application API:
- They are a compartmentalized “container” for the values in a JSON string that the UI sends as a parameter to the API.
- They are a compartmentalized container for the values in a row of a `GET` stored procedure’s result, having a variable that corresponds to each column selected; results with multiple rows are stored as an IEnumerable of view models with one model per row.
- They define which variables correspond to which columns to be inserted into or updated in an `INSERT` or `UPDATE` stored procedure.

All view models related to the apartment application begin with the word `Apartment`.

### Database Tables

Data specifically intended for the apartment application is stored in the CCT database in a series of tables whose names begin with `AA_`.

The details of these tables can be viewed and edited in SQL Server Management Studio with the right permissions. 
Although, a faster method for simply viewing the tables is to open `Gordon360/Models/CCT_DB_Models.edmx` in Visual Studio 2017 and pan the screen around until the box for the desired table is in view. In the edmx, the type and nullability of each attribute of a table can also be viewed by clicking a column name and looking at the Properties table in the bottom right corner of Visual Studio 2017.

#### CCT Tables

All the tables were created from scratch by our team.

##### AA_Admins
- AdminID - varchar(10)

| AdminID  |
|----------|
|222222222 |
|333333333 |

##### AA_ApartmentApplications
- AprtAppID - int, not nullable
- DateSubmitted - datetime, nullable
- DateModified - datetime, not nullable
- EditorUsername - varchar(50), not nullable

| AprtAppID | DateSubmitted           | DateModified            | EditorUsername     |
|-----------|-------------------------|-------------------------|--------------------|
| 1         | NULL                    | 2021-01-01 12:12:12.111 | firstname.lastname |
| 2         | 2021-01-01 12:12:12.111 | 2021-01-01 12:12:12.111 | anthony.aardvark   |

##### AA_ApartmentChoices
- AprtAppID - int, not nullable
- Ranking - int, not nullable
- HallName - varchar(15), not nullable

| AprtAppID | Ranking | HallName |
|-----------|---------|----------|
| 1         | 1       | Tavilla  |
| 1         | 2       | Bromley  |
| 2         | 1       | Tavilla  |
| 2         | 2       | Bromley  |

##### AA_ApartmentHalls
- Name - varchar(15), not nullable

| Name     |
|----------|
| Bromley  |
| Conrad   |
| Hilton   |
| MacInnis |
| Rider    |
| Tavilla  |

##### AA_Applicants
- AprtAppID - PK, int, not nullable
- Username - PK, varchar(50), not nullable
- AprtProgram - varchar(50), nullable
- AprtProgramCredit - bit, nullable
- SESS_CDE - char(8), not nullable

| AprtAppID | Username           | AprtProgram | AprtProgramCredit | SESS_CDE |
|-----------|--------------------|-------------|-------------------|----------|
| 1         | firstname.lastname |             | 0                 | 202101   |
| 2         | anthony.aardvark   |             | 0                 | 202101   |


### Stored Procedures

Stored Procedures are SQL scripts that are stored in the database. As it is with most of the 360 site’s features, the apartment application stored procedures can be found in `CCT/Programmability/Stored_Procedures/`. All the ones related to the apartment application have the acronym `AA` in their name after the word indicating what type of procedure they are (INSERT_AA_..., GET_AA_…, etc.). Each procedure’s description and code can be viewed by right-clicking its name and choosing `Modify` from the menu.


## Where to Find the Code for X

Note: This part of the documentation is the most vulnerable to becoming outdated! 

- __The route to call in order to perform a function in the backend__ 
  - In other words, what would X be here in your frontend code: `result = await http.get(`housing/X/`);` 
  - Just look above the declaration of the Housing Controller method you want to call to find something of the form `[Route("X")]` 
  - Just be careful that the the line above that, which is either `[HttpGet]`, `[HttpPut]`, `[HttpPost]`, or `[HttpDelete]`, corresponds to the type of operation you want to perform
  - If part of X has `{` and `}` around it, that is a parameter, and you have to do `result = await http.get(`housing/someRoute/${valuePassingIn}`);`
- __The identity and data type of any parameters that must be passed from the frontend__
  - There are two kinds of parameters that the frontend must supply, those that should be named in the route and those that should be placed in the body of the HTTP request 
  - Those that should be named in the route are identified in the same place as the previous point, `[Route("someRoute/{X}")]` in the respective Housing Controller method
  - The data type for these, let's call it Y, is identified next to the corresponding parameter in the actual method declaration: `public IHttpActionResult DoSomething(Y X)` 
    - Since these "inline" parameters are passed in the form of text that is part of the URL, the data type is limited to primitive types, such as `string` or `int` for example.
  - Those parameters that should be placed in the body of the frontend's request are only mentioned in the method declaration and are preceded by their data type and the phrase `[FromBody]`.
  - These `[FromBody]` parameters can sometimes have a complex data type called a View Model
    - In Visual Studio, you can right-click on the data type and choose "Go To Definition" to see what it entails
    - You will find that it is essentilly a bundle of parameters with various data types; that means make sure you are sending it a JSON string with all the necessary parameters
    - You can also find the definition by looking for the file with the same name as the parameter's type in the View Models folder 
    
- __The code that validates the input__
  - Each Housing Controller method has the same block of code that begins `if (!ModelState.IsValid ...`
    - This `ModelState` check is only needed if the controller method accepts a View Model as one of its parameters.
  - This, as far as the author of this documentation knows, simply throws a `BadInputException` if any parameter is null, empty, or not matching the View Model
- __How the code gets the information of the logged-in user__
  - This is done with methods of Account Service within each Housing Controller method that requires this information 
- __The code that is used to figure out whether to show admin or applicant view__
  - `public bool CheckIfHousingAdmin(...)` in the Housing Service
- __Where we get the list halls for which students can apply__
  - `public AA_ApartmentHalls[] GetAllApartmentHalls()` in the Housing Service, which just gets all the rows from a table called `AA_ApartmentHalls` in the CCT database
  - this table can only be edited by editing rows directly; there are no stored procedures for this
- __The code that figures out which application to load for a returning user__
  - This is also used to check if a student is already on another existing application 
  - `public int? GetApplicationID(...)` in the Housing Service
- __The code that saves an application__
  - `public int SaveApplication(...)` in the Housing Service
  - This covers saving everything: the applicants, their off-campus program, the application ID (which is autogenerated in the database), and the ranking of halls applied for
- __The code that edits an existing application__
  - `public int EditApplication(...)`
  - Though, The code that changes the application editor is separate, in `public bool ChangeApplicationEditor(...)` in the Housing Service
- __The code that gets the info of an existing application__
  - `public ApartmentApplicationViewModel GetApartmentApplication(...)` in the Housing Service
- __The code that gets all current applications and their info__
  - `public ApartmentApplicationViewModel[] GetAllApartmentApplication()` in the Housing Service
  - This info is only returned if the user is a housing admin
