# Gordon 360 API

The backend of Gordon 360, a Web API consumed by [gordon-360-ui](https://github.com/gordon-cs/gordon-360-ui).

## Table of Contents

- [The Code](#the-code)
  - [Introduction](#introduction)
- [Running the API locally](#running-the-api-locally)
  - [Preliminary setup](#preliminary-setup)
  - [Building and running](#building-and-running)
- [API Maintenance](#api-maintenance)
  - [Continuous Integration](#continuous-integration)
  - [Continuous Deployment](#continuous-deployment)
  - [Deploying Manually](#deploying-manually)


## The Code

### Introduction

This project is an ASP.NET Core Web API written in C#. It generallly follows the Model View Controller (MVC) pattern: the heart of the code is in `Controllers`, which are organized according to the REST API they implement. The `Model`s are auto-generated from the database objects using Entity Framework Core, and then `ViewModel`s are written to abstract the database structure into the RESTful API structure. The `View` layer is provided by the [frontend](https://github.com/gordon-cs/gordon-360-ui)

Here is a breakdown of the significant parts of the project folder:

- gordon-360-api/Gordon360/ - The main project. Most of the work will be done here.
  - Controllers/ - Folder containing the Controllers for the API endpoints.
  - Authorization/ - Contains code that enforces rules about who can access what.
  - browseable/ - Container for user-generated content (e.g. uploaded images). Only the placeholder content should be committed to git.
  - Documentation/ - All the comments in the code are concatenated and made into this file. This is an automatic ASP.NET feature.
  - Extensions/ - Extension methods to extend the behavior of built-in classes, e.g. new methods on `string`.
  - Exceptions/ - Custom exceptions that are thrown in the code. These exceptions get thrown instead of the default 500 Server Error Exception.
  - Models/ Contains auto-generated models of database objects atd custom ViewModels for abstraecting over the database objects.
  - Properties/ - Contains files that Visual Studio uses to build and publish the project. No need to dig into this unless you want to fine tune the build process.
  - Services/ - Services that are used by the Controller. The concept of a Service is also a design pattern. It is very useful for decoupling. (e.g. Making sure Controller code is separate from code that accesses the database).
  - Static Classes/ - Helper classes that are used throughout the code.

When the API receives an HTTP Request, it is routed to the Controller method with the matching `Route` attribute. For example, a HTTP Get request for `https://360api.gordon.edu/api/activities` will be routed to the `ActivitesController.Get` method. The Controller will make use of one or more Services (e.g. the ActivityService) to get data from the database as models/viewmodels. The controller will return that data, which will be serialized to JSON as the body of the HTTP Response. 

## Running the API locally

### Preliminary setup

- There is a dedicated development virtual machine to work on this project. Follow [these instructions](docs/RemoteDesktopToVM.md##How-to-connect-to-a-CPS-Server-virtual-machine) to set up and connect to the virtual machine using your Gordon account.

- Open the project in Visual Studio (*Note:* **not** Visual Studio *Code*, but Visual Studio 2022 or later):

  - Look for the desktop app Visual Studio, which has a purple Visual Studio icon. You might have to search for it through the start menu. You will have to log in to a Microasoft of account. Your Gordon email will work for this. 

  - If this is your first time on the virtual machine, you will need to clone this repository. In Visual Studio, select the "Clone a repository" option from right-hand side of the start window (or go to `File > Clone Repository`). Select GitHub from the "Browse a repository" section. Find the `gordon-cs/gordon-360-api` repository (you may need to sign in to GitHub). Select `Clone`. This will clone the repository from GitHub and open it in Visual Studio.

- Before you can run the project, you need to configure it:

  - **Copy the `appsettings.Development.json` file.** This file contains environment variables used to configure the application for local development. On the VM, it can be found in the folder `C:\360ConfigFiles`. Copy it to the same folder as the default `appsettings.json` file in the project. To find that folder in Visual Studio, look in the solution explorer on the right and open the `Gordon360` folder.  Right-click on `appsettings.json` and select "Open Containing Folder".

  - **Set non-SSL an SSL port numbers.** The front-end (UI) code connects with the back-end (API) through two ports. These should be chosen to be different than the ports used by the Production and Train servers, and should also be different than ports used by other developers. Follow any instructions given to you about choosing port numbers. To set them in Visual Studio, use the solution explorer on the right and open `Gordon360 > Properties > launchSettings.json`.
  
    - Find the `applicationUrl` property in the `Development` section of `profiles`. and update the string assigned to it to use your chosen port numbers. For example, if you chose ports 51620 (for non-SSL `http://`) and 51621 (for SSL `https://`), change `applicationUrl` to `"https://localhost:51621;http://localhost:51620"`.
    - A few lines above, edit the `launchUrl` string (still within the `Development` section) to use your chosen SSL port number.  In our example the string becomes `"https://localhost:51621/swagger"`.

### Building and running

- Now, you can press the Start button in Visual Studio to run the server (it is a green play button in the top middle of the tool bar).   If you get an error, double click on **Gordon360.sln** in the solution explorer.

- A web browser will open. If you get a "Your connection isn't priviate" warning, click "Advanced" and then "Continue to locahost (unsafe)".  You should see the Swagger page. Swagger is a tool that lets you send HTTP requests to each API Endpoint defined in the project.

- If you want to test the UI, keep the server running and follow the directions found [here](https://github.com/gordon-cs/gordon-360-ui/blob/develop/docs/Developer's%20Guide.md#connecting-to-the-backend).

## API Maintenance

### Continuous Integration

The backend uses GitHub Actions for Continuous Integration. Whenever changes are pushed to GitHub, the `CI` action defined in `.github/workflows/ci.yml` will be run by GitHub Actions. This action will checkout the latest version of whatever branch was pushed to and attempt to build it. If the build fails, the action will fail and no pull request on that branch will be able to be merged until a new build succeeds. This is the minimum in Continuous Integration. Ideally, we would also have unit/integration tests that run in the `CI` action. We currently have some tests defined in the `pytest` Python format, which can be found in `Tests/ApiEndpoints`. These are difficult to use in a Continuous Integration system because they require the API-to-be-tested to be running and locally accessible to the `pytest` module.

### Continuous Deployment

The Gordon 360 API is hosted on the `360api.gordon.edu` server. The built files are deployed at `D:\Sites`, under the names `360Api` and `360ApiTrain` for the `master` and `develop` branches respectively.

The backend is deployed automatically using GitHub Actions. Whenever changes are pushed to the `develop` or `master` branches, the `CI` workflow will run and ensure a successful build (see above [section on Continuous Integration](#continuous-integration) for details). If the changes build successfully, the output of that build will be saved as an artifact on the workflow run - `build-Train` for `develop` and `build-Prod` for `master`.

To detect and deploy successful builds, the scheduled task `Deploy 360Api[Train]` runs every 5 minutes on the API server. It calls the powershell script `Deploy360BackEnd.ps1` (found at `F:\Scripts\Deploy`), polling GitHub's API for new builds. If it detects a new build for the relevant environment (`Train` for `develop`, `Prod` for `master`), it will backup the existing API and deploy the new one. Transcripts for these deployments can be found at `F:\Scripts\Deploy\Transcripts`.

The API server publishes its build timestamp and github SHA on the /Version endpoint, so checking this with Swagger (e.g., https://360apitrain.gordon.edu/swagger/) is a good way to see when the new version is deployed and serving.

### Deploying Manually

If there are problems with continuous deployment, or a specific need arises to revert or push manually, then this older procedure can be used.

- Access the 360api.gordon.edu VM (see [RemoteDesktopToVM.md](docs/RemoteDesktopToVM.md#How-to-connect-to-a-CS-RDSH-virtual-machine) for instructions).
- Make a backup of the current stable version:
  - Navigate in File Explorer to `F:\Sites` and copy either 360Api or 360ApiTrain, whichever you're planning to publish to.
  - Paste that copy in the same place (`F:\Sites`), and rename it to a backup including the date. For example, if you backed up the Train site on January 1, 2001, then the copy would be named `360ApiTrain-backup-2001-01-01`.
- TODO: Document how to deploy manually post-restructuring

## Additional Documentation

The `docs` folder contains further documentation. In particular, the [Database](docs/Database.md) and [Endpoints](docs/Endpoints.md) documents may be useful to anyone trying to learn more about the project. However, both of these documents are known to be outdated, lacking important info, and in some cases fully incorrect. We hope to update/replace them sometime soon.
