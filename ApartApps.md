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


### Stored Procedures

Stored Procedures are SQL scripts that are stored in the database. As it is with most of the 360 site’s features, the apartment application stored procedures can be found in `CCT/Programmability/Stored_Procedures/`. All the ones related to the apartment application have the acronym `AA` in their name after the word indicating what type of procedure they are (INSERT_AA_..., GET_AA_…, etc.). Each procedure’s description and code can be viewed by right-clicking its name and choosing `Modify` from the menu.

