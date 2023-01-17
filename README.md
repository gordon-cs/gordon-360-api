# Gordon 360 API

The backend of Gordon 360, a Web API consumed by [gordon-360-ui](https://github.com/gordon-cs/gordon-360-ui). This project is ASP.NET Core Web API written in C#.

Dive in.

## Table of Contents

- [API Maintenance](#api-maintenance)
  - [Continuous Integration](#continuous-integration)
  - [Continuous Deployment](#continuous-deployment)
  - [Deploying Manually](#deploying-manually)
- [Running the API locally](#running-the-api-locally)
  - [Preliminary setup](#preliminary-setup)
  - [Building and running](#building-and-running)
- [The Database](#the-database)
  - [Tables](#CCT-tables)
  - [Stored Procedures](#CCT-stored-procedures)
  - [Triggers](#CCT-triggers)
  - [Manual and Debugging Access](#manual-and-debugging-access)
  - [Updating or creating .edmx](#updating-or-creating-edmx)
- [The Code](#the-code)
  - [Introduction](#introduction)
  - [Adding New Queries](#adding-new-queries)
- [Documentation](#documentation)

## API Maintenance

### Continuous Integration

The backend uses GitHub Actions for Continuous Integration. Whenever changes are pushed to GitHub, the `CI` action defined in `.github/workflows/ci.yml` will be run by GitHub Actions. This action will checkout the latest version of whatever branch was pushed to and attempt to build it. If the build fails, the action will fail and no pull request on that branch will be able to be merged until a new build succeeds. This is the minimum in Continuous Integration. Ideally, we would also have unit/integration tests that run in the `CI` action. We currently have some tests defined in the `pytest` Python format, which can be found in `Tests/ApiEndpoints`. These are difficult to use in a Continuous Integration system because they require the API-to-be-tested to be running and locally accessible to the `pytest` module.

### Continuous Deployment

The Gordon 360 API is hosted on the `360api.gordon.edu` server. The built files are deployed at `D:\Sites`, under the names `360Api` and `360ApiTrain` for the `master` and `develop` branches respectively.

The backend is deployed automatically using GitHub Actions. Whenever changes are pushed to the `develop` or `master` branches, the `CI` workflow will run and ensure a successful build (see above [section on Continuous Integration](#continuous-integration) for details). If the changes build successfully, the output of that build will be saved as an artifact on the workflow run - `build-Train` for `develop` and `build-Prod` for `master`.

To detect and deploy successful builds, the scheduled task `Deploy 360Api[Train]` runs every 5 minutes on the API server. It calls the powershell script `Deploy360BackEnd.ps1` (found at `F:\Scripts\Deploy`), polling GitHub's API for new builds. If it detects a new build for the relevant environment (`Train` for `develop`, `Prod` for `master`), it will backup the existing API and deploy the new one. Transcripts for these deployments can be found at `F:\Scripts\Deploy\Transcripts`.

### Deploying Manually

If there are problems with continuous deployment, or a specific need arises to revert or push manually, then this older procedure can be used.

- Access the 360api.gordon.edu VM (see [RemoteDesktopToVM.md](docs/RemoteDesktopToVM.md#How-to-connect-to-a-CS-RDSH-virtual-machine) for instructions).
- Make a backup of the current stable version:
  - Navigate in File Explorer to `F:\Sites` and copy either 360Api or 360ApiTrain, whichever you're planning to publish to.
  - Paste that copy in the same place (`F:\Sites`), and rename it to a backup including the date. For example, if you backed up the Train site on January 1, 2001, then the copy would be named `360ApiTrain-backup-2001-01-01`.
- TODO: Document how to deploy manually post-restructuring

## Running the API locally

### Preliminary setup

- It is easiest to use the development virtual machinea to of work on this project. Follow [these instructions](RemoteDesktopToVM.md#How-to-connect-to-a-CS-RDSH-virtual-machine) to set up and connect to the virtual machine using your Gordon account.

- Open the project in Visual Studio

  - Look for the desktop app Visual Studio, which has a purple Visual Studio icon. You might have to search for it through the start menu. You will have to log in to a Microasoft of account. Your Gordon email will work for this. 

  - If this is your first time on the virtual machine, you will need to clone this repository. In Visual Studio, select the "Clone a repository" option from right-hand side of the start window (or go to `File > Clone Repository`). Select GitHub from the "Browse a repository" section. Find the `gordon-cs/gordon-360-api` repository (you may need to sign in to GitHub). Select `Clone`. This will clone the repository from GitHub and open it in Visual Studio.

- Before you can run the project, you need to configure it:

  - You will need a copy of the `appsettings.Development.json` file. This file contains environment variables used to configure the application for local development. On the VM, it can be found at the path `D:\`. Copy it to the same folder as the default `appsettings.json` file in the project. To find that folder, from Visual Studio, right-click on `appsettings.json` and select "Open Containig Folder".

  - When running the project on the shared VMs, you need to make sure that runs on a different port than everyore else using that machine. In the solution explorer on the right, open `Gordon360 > Properties > launchSettings.json`. Edit the `profiles.Development.applicationUrl` property so it contains a port that is unused on the machine. For example, if you chose port 5555, change `applicationUrl` to `"http://localhost:5555"`. Make sure to edit the `profiles.Development.launchUrl` setting to the same Port number.

### Building and running

- Now, you can press the Start button in Visual Studio to run the server (it is a green play button in the top middle of the tool bar). It will open the web browser and load the Swagger page. Swagger is a tool that lets you send HTTP requests to each API Endpoint defined in the project.

- If you want to test the UI, keep the server running and follow the directions found [here](https://github.com/gordon-cs/gordon-360-ui/blob/develop/README.md#connect-local-backend-to-react) under "Connect Local Backend to React".


## The Code

### Introduction

The server was written using ASP.NET and is generally structured as such. As a MVC (Model View Controller) system, the heart of the code is in ApiControllers (which is organized like the API it implements, which is documented later in this file) and in the Models folder. The View is provided by a separate repository, gordon-360-ui.

Here is a breakdown of the project folder:

- gordon-360-api/
  - Design_Documents/ - currently empty. I do not actually remember why we had this.
  - Gordon360/ - The main project. Most of the work will be done here.
    - ApiControllers/ - Folder containing the Controllers for the API endpoints.
    - AuthorizationFilters/ - Contains code that enforces rules about who can access what.
    - AuthorizationServer/ (The folder should really be called AuthenticationServer) - Contains code that does user authentication.
    - bin/ - binary files. nothing to see here.
    - browseable/ - Placeholder folder that will be moved AS-IS to the built product. It will end up containing user-generated content. Right now, it only contains uploaded activity pictures.
    - Documentation/ - All the comments in the code are concatenated and made into this file. This is an automatic ASP.NET feature. We were going to somehow use the generated file for documentation, but didn't go through with it.
    - Exceptions/ - Custom exceptions that are thrown in the code. These exceptions get thrown instead of the default 500 Server Error Exception.
    - Models/ Code for the models (Model as in MODEL-View-Controller, or MVC)
    - obj/ - Not really sure what this is. It is also automatically generated by ASP.NET.
    - Properties/ - Contains files that Visual Studio uses to build and publish the project. No need to dig into this unless you want to fine tune the build process.
    - Repositories/ - Contains Repository files and Unit of Work Files. Both of these are object-oriented design patterns. They help with separation of concerns and general code organization.
    - Services/ - Services that are used by the ApiController. The concept of a Service is also a design pattern. It is very useful for decoupling. (e.g. Making sure Controller code is separate from code that accesses the database).
    - Static Classes/ - Helper classes that are used throughout the code.
    - Stored Procedures/ - This folder can be deleted. It is a relic from the past when I used to hard-code my stored procedures.
  - packages/ - ASP.NET packages that the project depends on. You will not be making any changes here.
  - Tests/ - Folder for tests
    - ApiEndpoints/ - I talk about this more in the Testing section.

### Adding New Queries

- (\*) is your new Title (ex: Membership, Account, Session)
- (+) is your new stored procedure name (ex: MEMBERSHIPS_PER_STUDENT_ID)
- New Files:
  - \*Controller.cs under ApiControllers
    - create new route
    - calls the \*service function
    - returns ok
  - \*Service.cs under Services
    - calls the stored procedure that returns view model
  - \*ViewModel.cs under ViewModels
    - function names correspond to the columns of the data the stored procedure returns (Does not have to be exact names)
    - static implicit operator converts * model to *ViewModel
- Update Files:
  - ServiceInterfaces.cs under Services
    - Add a public interface I\*/
    - Add all functions you have in \*Service under this interface
  - IUnitOfWork.cs under Repositories
    - Make corresponding IRepository for \* (ex. IRepository<STUDENTEMPLOYMENT> StudentEmploymentRepository {get;})
  - UnitOfWork.cs under Repositories
    - Make private IRepository<\*> variable
    - Write public function called \*Repository
  - Names.cs under Static Classes
    - Add public const string \*
