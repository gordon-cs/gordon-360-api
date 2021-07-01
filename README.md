# Gordon 360

#### The API consumed by [gordon-360-ui](https://github.com/gordon-cs/gordon-360-ui)

Dive in.

## Table of Contents

-   [Machines and Sites](#machines-and-sites)
-   [API Maintenance](#api-maintenance)
    -   [Continuous Integration](#continuous-integration)
    -   [Continuous Deployment](#continuous-deployment)
    -   [Deploying Manually](#deploying-manually)
-   [Caching](#caching)
-   [Running the API locally](#running-the-api-locally)
    -   [Preliminary setup](#preliminary-setup)
    -   [Building and running](#building-and-running)
-   [The Database](#the-database)
    -   [Tables](#CCT-tables)
    -   [Stored Procedures](#CCT-stored-procedures)
    -   [Triggers](#CCT-triggers)
    -   [Manual and Debugging Access](#manual-and-debugging-access)
    -   [Updating or creating .edmx](#updating-or-creating-edmx)
-   [The Code](#the-code)
    -   [Introduction](#introduction)
    -   [Adding New Queries](#adding-new-queries)
-   [Caching](#caching)
-   [API Endpoints](#api-endpoints)
    -   [Accounts](#accounts)
    -   [Activities](#activities)
    -   [Admins](#admins)
    -   [Advanced Search](#advanced-search)
    -   [Authentication](#authentication)
    -   [Content Management](#content-management)
    -   [Emails](#emails)
    -   [Events](#events)
    -   [Housing](#housing)
    -   [Memberships](#memberships)
    -   [Membership Requests](#membership-requests)
    -   [Participation Definitions](#participation-definitions)
    -   [Profiles](#profiles)
    -   [Sessions](#sessions)
    -   [Dining](#dining)
    -   [Student Employment](#student-employment)
    -   [Schedule](#schedule)
    -   [MySchedule](#my-schedule)
    -   [ScheduleControl](#schedule-control)
    -   [Victory Promise](#victory-promise)
    -   [News](#news)
    -   [Wellness Check](#wellness-check)
-   [API Testing](#api-testing)
    -   [Testing Introduction](#testing-introduction)
    -   [Running the Tests](#running-the-tests)
    -   [Writing the Tests](#writing-the-tests)
-   [Troubleshooting](#troubleshooting)
-   [Documentation](#documentation)

## Machines and Sites

As of Spring 2021, the virtual machines CS-RDSH-01 and CS-RDSH-02 are used for developing Gordon 360. Instructions for connecting via Remote Desktop can be found in [RemoteDesktopToVM.md](RemoteDesktopToVM.md#How-to-connect-to-a-CS-RDSH-virtual-machine).

## API Maintenance

### Continuous Integration

The backend uses GitHub Actions for Continuous Integration. Whenever changes are pushed to GitHub, the `CI` action defined in `.github/workflows/ci.yml` will be run by GitHub Actions. This action will checkout the latest version of whatever branch was pushed to and attempt to build it. If the build fails, the action will fail and no pull request on that branch will be able to be merged until a new build succeeds. This is the minimum in Continuous Integration. Ideally, we would also have unit/integration tests that run in the `CI` action. We currently have some tests defined in the `pytest` Python format, which can be found in `Tests/ApiEndpoints`. These are difficult to use in a Continuous Integration system because they require the API-to-be-tested to be running and locally accessible to the `pytest` module.

### Continuous Deployment

The Gordon 360 API is hosted on the `360api.gordon.edu` server, which is also known as `cts-360.gordon.edu`. The built files are deployed at `F:\Sites`, under the names `360Api` and `360ApiTrain` for the `master` and `develop` branches respectively.

The backend is deployed automatically using GitHub Actions. Whenever changes are pushed to the `develop` or `master` branches, the `CI` workflow will run and ensure a successful build (see above [section on Continuous Integration](#continuous-integration) for details). If the changes build successfully, the output of that build will be saved as an artifact on the workflow run - `build-Train` for `develop` and `build-Prod` for `master`.

To detect and deploy successful builds, the scheduled task `Deploy 360Api[Train]` runs every 5 minutes on the API server. It calls the powershell script `Deploy360BackEnd.ps1` (found at `F:\Scripts\Deploy`), polling GitHub's API for new builds. If it detects a new build for the relevant environment (`Train` for `develop`, `Prod` for `master`), it will backup the existing API and deploy the new one. Transcripts for these deployments can be found at `F:\Scripts\Deploy\Transcripts`.

### Deploying Manually

If there are problems with continuous deployment, or a specific need arises to revert or push manually, then this older procedure can be used.

-   Access the cts-360.gordon.edu VM (see [RemoteDesktopToVM.md](RemoteDesktopToVM.md#How-to-connect-to-a-CS-RDSH-virtual-machine) for instructions) as the cct.service user.
-   Before you publish your new version, be sure to copy the current stable version to make a backup. To do so:
    -   Navigate in File Explorer to `F:\Sites` and copy either 360Api or 360ApiTrain, whichever you're planning to publish to.
    -   Paste that copy in the same place (`F:\Sites`), and rename it to a backup including the date. For example, if you backed up the Train site on January 1, 2001, then the copy would be named `360ApiTrain-backup-2001-01-01`.
-   Open gitbash and cd to `C:\users\cct.service\code\gordon-360-api`. Make sure that you are on the branch you wish to deploy, and that it has been pulled up to date.
    **Note: if you clone a new repository on this VM, it will not have the necessary publish profiles or secrets.config. See [MakePublishProfiles.md](MakePublishProfiles.md#How-to-create-the-Publish-Profiles-to-publish-the-API-to-the-Sites) to restore the Publish Profiles.**
-   Start Visual Studio as an administrator (right click) and open the existing project/solution file - `C:\users\cct.service\code\gordon-360-api\Gordon360.sln` (the solution file).
-   Menu Bar -> Build - Publish Gordon360.
-   Choose the right publish profile.
    -   DEV -- Development ( Connects to the admintrainsql database server, and used for 360train.gordon.edu).
    -   Prod -- Production ( Connects to the adminprodsql database server, and used for the real site 360.gordon.edu).
    -   If you don't see the publish profile you want (or you are automatically taken to the "Pick a Publish Target" Window) see [MakePublishProfiles.md](MakePublishProfiles.md#How-to-create-the-Publish-Profiles-to-publish-the-API-to-the-Sites) to restore the Publish Profiles.
-   Clicking publish pushes your changes to the API for either 360ApiTrain.gordon.edu or 360Api.gordon.edu, depending on which publish profile you used.

## Caching

Since the type of solution we are using does not run like many systems, we have to cache a request that occurs every few minutes after startup. As such, we have implemented code in the startup.cs file that:
1 ) Performs static methods and saves the output to a static object (located in Helpers and Data, respectively)
2 ) Create an entry in the cache that then runs these static methods every few minutes

This process makes use of static names, methods, and data, since ASP.NET does not use global variables.

Data which is stored upon startup includes:

-   All events in 25Live ending after the start of the current academic year
-   All basic information on every account with an AD Username
-   All student, faculty, staff and alumni profile info

## Running the API locally

### Preliminary setup

-   It is easiest to use the development virtual machine to work on this project. Follow [these instructions](RemoteDesktopToVM.md#How-to-connect-to-a-CS-RDSH-virtual-machine) to set up and connect to the virtual machine using your Gordon account.

-   If this is your first time on the virtual machine, you will need to clone this repository. You can do this by using Git Bash. It is possible that you will need to [add the SSH key to your Git account](https://help.github.com/en/github/authenticating-to-github/connecting-to-github-with-ssh), it will guide you to `cd .ssh` then `cat id_rsa.pub`, copy the output and go to your github settings and paste it in your SSH keys.

    -   Before you open the gordon-360-api folder, you will have to add the `secrets.config` file to it. The file is located on the CS-RDSH-02 virtual machine in `C:\Users\Public\Public Documents\` (or `/c/users/public/documents\` when in git-bash). Copy the file `secrets.config` to the same folder in your project that contains the `web.config` file; currently, this is in `gordon-360-api\Gordon360`. This file is a sort of keyring for the server to authorize itself at various points. You can do this by CDing into the Gordon360 folder and typing `cp /C/users/public/documents/secrets.config .`

    -   Look for the desktop app Visual Studio 2017, which has a purple Visual Studio icon. You might have to search for it through the start menu. You will have to log in to a Microsoft account. Your Gordon email will work for this. Once you log in, go to `File > Open > Project/Solution`. In the navigation box that pops up, navigate to the directory where you cloned this repo, and select and open the file `/Gordon360.sln`.

    -   In the solution explorer on the right, right click the name of the project (Gordon360) and select properties. From the tabs on the left, choose the Web tab and change the Project Url so it contains a port that is unused on the machine. For example, if you chose port 5555, change Project Url to `"http://localhost:5555"`. Then click Create Virtual Directory. Make sure that the protocol is `http`, not `https`. Press OK on the dialog box, and you all configured!

### Building and running

-   Now, you can press the Start button in Visual Studio to run the server (it is a green play button in the top middle of the tool bar). It will open the web browser and, after a period that should just be a few minutes long but sometimes lasts half an hour or more, display an Error 403.14 - Forbidden. This is expected. You can now begin manually testing the API.

-   If you haven't already, you'll need to `npm install` in the gordon-360-ui repo. This only needs to be done once, then you can just `npm start`.

-   If you want to test the UI, keep the server running and follow the directions found [here](https://github.com/gordon-cs/gordon-360-ui/blob/develop/README.md#connect-local-backend-to-react) under "Connect Local Backend to React".

## The Database

The `CCT` and `MyGordon` databases exist in:

-   `admintrainsql.gordon.edu` - The development/test database server
-   `adminprodsql.gordon.edu` - The production/live database server.

### CCT Tables

All the tables were created from scratch by our team.

Misc Information:

-   Apart from a few exceptions, the tables don't make use of foreign key constraints. This is because the relevant primary keys are in the tables referenced by Views. Unfortunately, one cannot make foreign keys that reference Views.

###### ACT_INFO

A record in this table stores:

-   ACT_CDE - The short code for the activity.
-   ACT_DESC - The name of the activity.
-   ACT_BLURB - A short description of what the activity is about. This will be filled out by a leader.
-   ACT_URL - URL to the website for the club/organization (if they have one).
-   ACT_IMAGE_PATH - Path to where the activity logo is stored in the browseable folder.
-   ACT_TYPE - Short code for the type of the activity
-   ACT_TYPE_DESC - Full name of the type of the activity

You might notice that this table is an extension of the ACT_CLUB_DEF view. It contains extra information that the view does not have, but that we need. This is clearly a case of Information Duplication; information is available in two places and can easily fall out of sync. To remedy this, the stored procedure [UPDATE_ACT_INFO](#update_act_info) was made.

###### ADMIN

A record in this table stores:

-   ID_NUM - The gordon id number of the administrator
-   USER_NAME - The administrator's username
-   EMAIL - The administrator's email.
-   SUPER_ADMIN - Whether or not the admin has super admin privilege

To make someone an admin, simply insert a record into this table through MSSQL Studio.

###### CUSTOM_PROFILE

A record in this table stores:

-   username - The gordon username of the current user (firstname.lastname)
-   facebook - The URL of the user's facebook without its domain name
-   twitter - The URL of the user's twitter without its domain name
-   instagram - The URL of the user's instagram without its domain name
-   linkedin - The URL of the user's linkedin without its domain name
-   handshake - The URL of the user's handshake without its domain name (just user id)

Users don't exist in the table unless they add/edit their social media links on 360 site. Once a user adds any links, the user will be added to the table. This logic is done so that there won't be unused users in the table which can possibly slow down the website.

###### MYSCHEDULE

A record in this table stores:

-   EVENT_ID - The event id number of this schedule (always has to be above 1000, to differentiate between a course schedule)
-   GORDON_ID - The gordon id number of the user having this event
-   LOCATION - The location of the event
-   DESCRIPTION - The description of the event
-   MON_CDE - Whether or not the event is in monday ('M')
-   TUE_CDE - Whether or not the event is in tuesday ('T')
-   WED_CDE - Whether or not the event is in wednesday ('W')
-   THU_CDE - Whether or not the event is in thursday ('R')
-   FRI_CDE - Whether or not the event is in friday ('F')
-   SAT_CDE - Whether or not the event is in saturday ('S')
-   SUN_CDE - Whether or not the event is in sunday ('N')
-   IS_ALLDAY - Whether or not the event is happening for all day '0' for no and '1' for all day.
-   BEGIN_TIME - The start time of the event in Timespan format
-   END_TIME - The end time of the event in Timespan format

Myschedules doesn't exist in the table unless a user add/edit myschedule on 360 site. Once a user adds any customized event, the event will be added to the table. The structure is adopted from the course schedule format stored in other database. There are two primary keys - EVENT_ID and GORDON_ID. They have to match together to get any event schedule

###### Schedule_Control

A record in this table stores:

-   IsSchedulePrivate - Whether or not the schedule is private (only applied to students and their course schedule. FacStaff and mySchedule won't be affected)
-   ModifiedTimeStamp - The last time when the user modified the event or description
-   Description - The schedule description for additional links
-   gordon_id - The gordon id number of the current user

Schedule Controls also don't exist in the table unless a user add/edit their settings on 360 site.

###### JNZB_ACTIVITIES

A record in this table stores all the same fields as an Activity table in Jenzabar would.
The goal of this table was to contain membership information that was to be moved to Jenzabar. To do this, one would use the stored procedure [UPDATE_JNZB_ACTIVITIES](#update_jnzb_activities).

###### MEMBERSHIP

A record in this table stores:

-   ACT_CDE - The short code for the activity.
-   SESS_CDE - The short code for the session when this membership took place.
-   ID_NUM - The gordon id number of the member.
-   PART_CDE - The short code for the participation level of the member.
-   BEGIN_DTE - The date the membership began
-   END_DTE - The date the membership ended
-   COMMENT_TXT - Comment about the membership
-   GRP_ADMIN - A boolean indicating whether or not this member has group admin privileges, allowing them to edit the group's page on the site

The other three fields (USER_NAME, JOB_NAME and JOB_TIME) where meant to be administrative fields to store data about who inserted records and when they did it. We ended up not using them. We kept them because they have good potential use.

###### REQUEST

A record in this table stores:

-   SESS_CDE - The short code for the session they are requesting to join.
-   ACT_CDE - The short code of the activity they are requesting to join.
-   ID_NUM - The gordon id number of the potential member.
-   PART_CDE - The short code for the participation level they want to join as.
-   DATE_SENT - The date the request was made.
-   COMMENT_TXT - Comments to accompany the request.
-   STATUS - Status of the request. Should be either Pending, Approved or Denied.\

###### ACT_CLUB_DEF

A record in this table stores

-   ACT_CDE - The activity short code.
-   ACT_DESC - The name of the activity.

This table is an exact duplicate of the JENZ_ACT_CLUB_DEF view. It is periodically updated by making sure what is in it corresponds to what is in JENZ_ACT_CLUB_DEF. When a new activity is found in JENZ_ACT_CLUB_DEF, it is inserted into ACT_CLUB_DEF and the stored procedure UPDATE_ACT_INFO is run.

### MyGordon Tables

These are just the MyGordon tables used by 360. They were originally not made for this site.

##### StudentNews

A record in this table stores

-   SNID - unique integer identifier for a news entry
-   ADUN - username (first.last) of the person who posted the entry
-   categoryID - foreign key that links this entry to its category
-   Subject - subject, written by the poster, of the news entry
-   Body - the actual text of the news entry, written by the poster
-   Accepted - whether this entry has been approved to be shown publicly
-   Sent - whether the item has been sent
-   thisPastMailing - whether it belongs to this past mailing
-   Entered - when, in datetime format, the post was submitted by the poster
-   fname - not used (NULL)
-   lname - not used (NULL)
-   ManualExpirationDate - given by the poster, the last day on which this entry should be displayed publicly

##### StudentNewsCategory

A record in this table stores

-   categoryID - a unique integer identifier
-   categoryName - the name of a category, ex. "Found Items"
-   SortOrder - an integer representing the category's placement in the preferred display order

### CCT Views

We got access to these views through CTS. They are a direct live feed from the tables they represent. As mentioned earlier, we cannot use primary keys in the views to make foreign keys in other tables.

###### ACCOUNT

Account information for all the members of gordon college.

###### Alumni

The Alumni information. Includes their information the same way as students.

###### Buildings

Descriptions of the different codes for buildings around campus.

###### CHAPEL_EVENT

Information on chapel attendance for every student

###### JENZ_ACT_CLUB_DEF

The Activity information. Includes short codes and what they represent.

###### Countries

Descriptions of different codes for countries.

###### CM_SESSION_MSTR

The Session information. Includes short codes, the session they represent, and the physical dates spanned by the session.

###### FacStaff

A subset of `ACCOUNT` that has only faculty and staff member records.

###### Majors

Descriptions of the different codes for majors.

###### ALL_BASIC_INFO

Pulls firstname, lastname, category (student, staff, faculty), and AD_Username (if it exists!) and then makes a concatenated string to be searched through

###### PART_DEF

Definitions of the different participation levels for someone in an activity.

###### Police

A list of IDs that are identified as gordon police.

###### Student

A subset of `ACCOUNT` that has only student records.

###### 360_SLIDER

Content (images, captions, and links) for the slider on the dashboard page.

### CCT Stored Procedures

Stored procedures have been written to make some database accesses and administrative tasks easier.
Here are the most important ones.

###### EVENTS_BY_STUDENT_ID

Returns all events which a student has attended based upon their AD_Username

###### ALL_BASIC_INFO

Pulls firstname, lastname, category (student, staff, faculty), and AD_Username (if it exists!) and then makes a concatenated string to be searched through

###### ADVISOR_SEPARATE

This stored procedures is simple. It returns separate advisor(s) ID (Max is 3, Advisor1, Advisor2, and Advisor 3) for a particular student based upon their ID.

###### UPDATE_ACT_CLUB_DEF
This keeps the ACT_CLUB_DEF table in sync with the JENZ_ACT_CLUB_DEF view. It should be run periodically.

###### UPDATE_ACT_INFO
Because ACT_INFO is basically a duplicate of ACT_CLUB_DEF, this stored procedure tries to keep them synced. Ideally it should be run automatically anytime ACT_CLUB_DEF changes.

In non-sql terms, this procedure makes sure all the activities defined in ACT_CLUB_DEF are also present in ACT_INFO. If something has been added/removed to ACT_CLUB_DEF but not in ACT_INFO, it adds/removes the corresponding record to ACT_INFO.

###### UPDATE_JNZB_ACTIVITIES
This stored procedures is pretty simple. It moves all the relevant information from the MEMBERSHIP table and puts it in the JNZB_ACTIVITIES table. To prevent duplication, it will only add records that are present in the MEMBERSHIP table, but missing the JNZB_ACTIVITIES table.

### CCT Triggers

###### ACT_CLUB_DEF_INSERT_TRIGGER

Every time a record is inserted into the ACT_CLUB_DEF table, this trigger runs the UPDATE_ACT_INFO stored procedure. Although not clear in the name, this trigger also runs whenever a row is deleted from ACT_CLUB_DEF.

### Manual and Debugging Access

It's sometimes useful to look at the database directly, to see the schema or check data. Here is how.

-   Use remote desktop to get to the Windows server VM
-   If SQL Server Management Studio is not pinned to the task bar, pin it by starting it and right clicking on it in the task bar to pin it)
-   Shift-right-click SSMS (SQL Server Management Studio) and select "Run as ..."
-   Run as "cct.service"
-   Connect to "ADMINTRAINSQL" database server (or "ADMINPRODSQL")
-   Expand "Databases" then "CCT" or "MyGordon" (the two currently used databases) then "Views"
-   To see schemas, expand "dbo." entries and their "columns"
-   To see data, right-click a view and select "Select top 1000 rows"

### Updating or Creating edmx

Everytime you update the one of the databases with a new table, column, view or stored procedure, or modify the existing ones with different parameters or return values, you need to get the corresponding Entity Database Model XML in the API. Editing it manually is not recommended, since it may cause unexpected errors such as PublicStudentData error.

<div style="background-color:#eee;padding:10px 20px">
<i><strong>Permissions Note:</strong></i>
<br>If data tables appears to be missing from edmx generation or if some kind of "SELECT permission denied" error occurs during testing, you may need to be running Visual Studio as the cct.service user (or obtain the appropriate permissions for yourself) before generating these files. (Summer 2020 note)</div>

#### UPDATE EDMX FILES

Visual Studio provides auto-generation of .edmx files, with the following procedure:

-   _Setup_

    -   Use remote desktop to get to the Windows server VM
    -   Open Visual Studio and load the solution file

-   _Deleting the old edmx files_

    -   In the solution explorer, expand the "Models" folder and delete the previous `<database name>_DB_MODELS.edmx` which represents the database to which you made changes; for example, if it was the CCT database that was changed, delete CCT_DB_MODELS.edmx by right clicking and selecting 'delete' (it's okay, we can remake it)
    -   At the bottom of the `web.config` file there is a `<connectionStrings>` tag with an `<add ...>` tag for every connection string nested within. Delete the connection string (the `<add ...>` tag) for the DB connection you are regenerating (if this is confusing, [this step](#connection-string) explains why you may need it deleted)

-   _Generating the new edmx files_
    -   Right-click "Models", expand "Add" and press "New Item" (If you can see ADO.NET Entity Data Model in here, you may press that as well)
    -   Under Visual C# panel, access "Data" and find ADO.NET Entity Data Model. Name it `<database name>_DB_Models` and create it
    -   In the Wizard, the default option should be "EF Designer from database". If it is not, change to this option and head next
    -   Choose the data connection "`<database name>Entities` (Gordon360)" (though, if it is CCT, there should be a "1" after "Entities"). If the appropriate connection does not exist, see <a href="#create-connection">below</a> to create one
    -   <span id="connection-string"><!--anchor--></span>
        Make sure you check "Save connection setting in Web.Config as:" and name the saved settings `<database name>Entities` (again, if it is CCT, it should be "CCTEntities1" with that additional "1"). If there is an error that the name already exists, it is because the connection string needs to be removed from `web.config` (see above)
    -   Next, you will see the wizard retrieving the database objects from our CCT database; check all boxes in the panel but you should uncheck the option "Pluralize or singularize generated object names"
    -   Name the Model Namespace as `Gordon360` and press finish

#### CREATE NEW EDMX FILES

If you are attempting to connect the API to a database other than the ones to which it is already connected, you will need to create an entirely new edmx. To do this, follow the instructions above for updating, taking note of the following points:

-   You do not need to delete any edmx files, since you are now creating the first instance of a different edmx
-   <span id="create-connection"><!--anchor--></span>
    None of the options for data connection will fit your needs, so you will need to create a new option:
    -   Click "New Connection..."
    -   If prompted "Choose Data Source", choose "Microsoft SQL Server"
    -   For "Server name", put `admintrainsql.gordon.edu`
    -   Under "Connect to a Database", make sure "Select or enter a database name:" is selected and enter `<database name>`
    -   Go to Advanced settings, scroll to the top, and make sure "MultipleActiveResultSets" is set to True; then, scroll towards the bottom to find "Integrated Security" and make sure that is set to True
    -   Click OK to close Advanced settings, then OK again to save the data connection you have just made
-   Now, you can select the data connection you just made from the drop down and pick up with the above directions at 'Make sure you check "Save connection...'

## The Code

### Introduction

The server was written using ASP.NET and is generally structured as such. As a MVC (Model View Controller) system, the heart of the code is in ApiControllers (which is organized like the API it implements, which is documented later in this file) and in the Models folder. The View is provided by a separate repository, gordon-360-ui.

Here is a breakdown of the project folder:

-   gordon-360-api/
    -   Design_Documents/ - currently empty. I do not actually remember why we had this.
    -   Gordon360/ - The main project. Most of the work will be done here.
        -   ApiControllers/ - Folder containing the Controllers for the API endpoints.
        -   AuthorizationFilters/ - Contains code that enforces rules about who can access what.
        -   AuthorizationServer/ (The folder should really be called AuthenticationServer) - Contains code that does user authentication.
        -   bin/ - binary files. nothing to see here.
        -   browseable/ - Placeholder folder that will be moved AS-IS to the built product. It will end up containing user-generated content. Right now, it only contains uploaded activity pictures.
        -   Documentation/ - All the comments in the code are concatenated and made into this file. This is an automatic ASP.NET feature. We were going to somehow use the generated file for documentation, but didn't go through with it.
        -   Exceptions/ - Custom exceptions that are thrown in the code. These exceptions get thrown instead of the default 500 Server Error Exception.
        -   Models/ Code for the models (Model as in MODEL-View-Controller, or MVC)
        -   obj/ - Not really sure what this is. It is also automatically generated by ASP.NET.
        -   Properties/ - Contains files that Visual Studio uses to build and publish the project. No need to dig into this unless you want to fine tune the build process.
        -   Repositories/ - Contains Repository files and Unit of Work Files. Both of these are object-oriented design patterns. They help with separation of concerns and general code organization.
        -   Services/ - Services that are used by the ApiController. The concept of a Service is also a design pattern. It is very useful for decoupling. (e.g. Making sure Controller code is separate from code that accesses the database).
        -   Static Classes/ - Helper classes that are used throughout the code.
        -   Stored Procedures/ - This folder can be deleted. It is a relic from the past when I used to hard-code my stored procedures.
    -   packages/ - ASP.NET packages that the project depends on. You will not be making any changes here.
    -   Tests/ - Folder for tests
        -   ApiEndpoints/ - I talk about this more in the Testing section.

### Adding New Queries

-   (\*) is your new Title (ex: Membership, Account, Session)
-   (+) is your new stored procedure name (ex: MEMBERSHIPS_PER_STUDENT_ID)
-   New Files:
    -   \*Controller.cs under ApiControllers
        -   create new route
        -   calls the \*service function
        -   returns ok
    -   \*Service.cs under Services
        -   calls the stored procedure that returns view model
    -   \*ViewModel.cs under ViewModels
        -   function names correspond to the columns of the data the stored procedure returns (Does not have to be exact names)
        -   static implicit operator converts * model to *ViewModel
-   Update Files:
    -   ServiceInterfaces.cs under Services
        -   Add a public interface I\*/
        -   Add all functions you have in \*Service under this interface
    -   IUnitOfWork.cs under Repositories
        -   Make corresponding IRepository for \* (ex. IRepository<STUDENTEMPLOYMENT> StudentEmploymentRepository {get;})
    -   UnitOfWork.cs under Repositories
        -   Make private IRepository<\*> variable
        -   Write public function called \*Repository
    -   Names.cs under Static Classes
        -   Add public const string \*

## API Endpoints

_Note:_ The shell script `get-route-list.sh` is run with `bash get-route-list.sh` from a linux shell or git-bash. It provides a list of the API routes that appear in the ApiController files.

### Accounts

What is it? Resource that represents a gordon account.

##### GET

`api/accounts/email/:email` Get the account with email `email`. Currently restricted to FacStaff, Police, and Group Admins.

`api/accounts/username/:username` Get the account with `username`. Currently restricted to FacStaff, Police, and Group Admins.

`api/accounts/search/:searchString` Returns the basicinfoviewmodel with a Concatenated attribute matching some or all of the searchstring

`api/accounts/search/:searchString/:secondaryString` The same as above, used when the search string contains a space

`api/accounts/advanced-people-search/{includeAlumniSearchParam}/{firstNameSearchParam}/{lastNameSearchParam}/{majorSearchParam}/{minorSearchParam}/{hallSearchParam}/{classTypeSearchParam}/{hometownSearchParam}/{stateSearchParam}/{countrySearchParam}/{departmentSearchParam}/{buildingSearchParam}` Get all the accounts matching the specified parameters. Access to accounts is based on your account type (e.g. Students can't get Alumni).

### Activities

What is it? Resource that represents some activity - such as a club, ministry, leadership program, etc.

##### GET

`api/activities` Get all the activities.

`api/activities/:id` Get the activity with activity code `id`.

`api/activities/session/:id` Get the activities offered during the session with session code `id`.

`api/activities/session/:id/types` Get the different activity types among the activities offered during the session with session code `id`.

`api/activities/{sessionCode}/{id}/status` Get the status of an activity (either open or closed), which indicates whether or not new members can be added to the activity for this session.

`api/activities/open` Get all the open activities for the current session.

`api/activities/:id/open` Get only the open activities for which a given user (identified by their user `id`) is the group admin.

`api/activities/closed` Get all the closed activities for the current session.

`api/activities/:id/closed` Get only the closed activities for which a given user (identified by their user `id`) is the group admin.

##### PUT

`api/activities/:id/session/{sess_cde}/close` Close out an activity for a given session (this is like confirming the final roster of an activity for a given session).

`api/activities/:id/session/{sess_cde}/open` Reopen an activity for a given session.

`api/activities/:id` Edit activity information for the club with activity code `id`.

`api/activities/:id/privacy/:p` Update a given activity to private or not private with boolean value `p`. The `id` parameter is the activity id.

##### POST

`api/activities/:id/image` Set an image for the activity with activity code `id`.

`api/activites/:id/image/reset` Reset the image to default for the activity with activity code `id`.

### Admins

What is it? Resource that represents admins.

Who has access? Only super admins, except to get a specific admin where all admins have access.

NOTE: facultytest is a super admins in PRODAPIDATA, stafftest is a super admins in TRAINAPIDATA

##### GET

`api/admins` Get all the admins.

`api/admins/:id` Get a specific admin with the Gordon ID specified.

##### POST

`api/admins` Create a new admin.

##### DELETE

`api/admins/:id` Delete the admin with the admin id `id`.

### Advanced Search

##### GET

`api/advanced-search/majors` Get all majors that are found in the Student table.

`api/advanced-search/minors` Get all minors that are found in the Student table.

`api/advanced-search/halls` Get all halls that are found in the Student table.

`api/advanced-search/states` Get all states that are found in the Student, Alumni, and FacStaff tables.

`api/advanced-search/countries` Get all countries that are found in the Student, Alumni, and FacStaff tables.

`api/advanced-search/departments` Get all the departments from the FacStaff table.

`api/advanced-search/buildings` Get all the buildings from the FacStaff table.

### Authentication

##### POST

`/token:` The authentication process uses a simplified version of what is called Open Authentication (OAuth).

In OAuth, there are two servers including the one running your app. The server running your app doesn't authenticate people directly, it relies on the second server to tell it if a given person is allowed access. This second server is called the Authentication Server.

In our project, the Authentication Server and the App Server are actually the same. They are only separated code-wise. You could say that the _App_ is the ApiControllers folder and the _Authentication Server_ is the AuthorizationServer folder (Recall that it is currently named incorrectly, sorry. We haven't changed the name because it would require changing multiple import statements, and we don't have time to debug after the change).

_**Note: See the section on <a href="#manual-test">Manually Testing the API</a> for most up-to-date instructions on authenticating your test HTTP requests.**_

Accepts a form encoded object in the body of the request:

```
{
	"username": YOUR-USERNAME,
	"password": YOUR-PASSWORD,
	"grant_type": "password"
}
```

Response will include an access token which should be included in subsequent request headers.
Specifically, include it in the `Authorization` header like so `Bearer YOUR-ACCESS-TOKEN`

### Content Management

What is it? Resource for fetching content that has been stored in the database by Gordon's website [content manager](http://wwwtrain.gordon.edu/).

##### GET

`api/cms/slider` Get the content for the dashboard slide.

### Emails

What is it? Resource that represents emails.

##### GET

`api/emails/activity/:id` Get the emails for members of the activity with activity code `id` during the current session.

`api/emails/activity/:id/session/:sessionid` Get the emails for the members of the activity with activity code `id` during the session with session code `sessionid`.

`api/emails/activity/:id/leaders` Get the emails for the leaders of the activity with activity code `id` during the current session.

`api/emails/activity/:id/leaders/session/:sessionid` Get the emails for the leaders of the activity with activity code `id` during the session with session code `sessionid`.

`api/emails/activity/:id/group-admin/session/:sessionid` Get the emails for the group admins of the activity with activity code `id` during the session with session code `sessionid`.

`api/emails/activity/:id/advisors` Get the emails for the advisors of the activity with activity code `id` during the current session.

`api/emails/activity/:id/advisors/session/:sessionid` Get the emails for the advisors of the activity with activity code `id` during the session with session code `sessionid`.

##### PUT

`api/emails` Sends an email.

`api/emails/activity/:id/session/:sessionid` Sends an email to the participants of the activity with activity code `id` during the session with session code `sessionid`.

`api/emails/activity/:id/leaders/session/:sessionid` Sends an email to the leaders of the activity with activity code `id` during the session with session code `sessionid`.

### Events

What is it? Resources to get information on Events from the 25Live system

-   Only confirmed events are pulled
-   Only events with event_type_id 14 `Calendar Announcement` or 57 `Event`.
-   Only events ending after the start of the current academic year are requested from 25Live
-   Data from 25Live is retrieved every four minutes using a cached request

##### GET

`api/events/chapel/:term` Get all events attended by the logged-in user in a specific term

`api/events/25Live/All` Returns all events in 25Live under predefined categories.

`api/events/25Live/CLAW` Returns all events in 25Live with Category_ID = 85 (CL&W Credit approved).

`api/events/25Live/Public` Returns all events in 25Live marked to promote on public calendars (Requirement_ID = 3).

### Housing
What is it? Resource that represents residence hall information that would concern the residents or housing director.

##### GET

`api/housing/admin` Gets an Http OK if the current user is in the admin whitelist table and gets Not Found otherwise

`api/housing/halls/apartments` Gets the list of apartment-style halls that are available for student housing and gets Not Found if it cannot be found

`api/housing/apartment/` Gets the application ID matching the current user if that user is on an application saved for the current semester, otherwise returns a Not Found code

`api/housing/apartment/${username}/` Gets the application ID matching the given username if that user is on an application saved for the current semester, otherwise returns a Not Found code

`api/housing/apartment/applications/{applicationID}/` Gets the apartment application info matching the given application ID if it is found in the database, otherwise returns a Not Found code

`api/housing/admin/apartment/applications` Gets the apartment application info for all applications submitted during the current semester, otherwise returns a Not Found code

##### PUT

`apartment/applications/{applicationID}/submit` Changes the date an application was submitted (changes it from null the first time they submit)

##### DELETE

`apartment/applications/{applicationID}` Deletes an application (and consequently all rows that reference it)



### Memberships

What is it? Resource that represents the affiliation between a student and a club.

##### GET

`api/memberships` Get all the memberships.

`api/memberships/:id` Get the membership with membership id `id`.

`api/memberships/activity/:id` Get the memberships associated with the activity with activity code `id`.

`api/memberships/activity/:id/leaders` Get the memberships of the leaders for the activity with activity code `id`.

`api/memberships/activity/:id/advisors` Get the memberships of the advisors for the activity with activity code `id`.

`api/memberships/activity/:id/group-admin` Get the memberships of the group admin (displayed as "Group Contacts") for the activity with activity code `id`.

`api/memberships/activity/:id/followers` Get the number of followers of an activity with activity code `id`.

`api/memberships/activity/:id/members` Get the number of members (excluding followers) of an activity with activity code `id`.

`api/memberships/activity/:id/followers/:sess_cde` Get the number of followers of an activity with activity code `id` in session `:sess_cde`.

`api/memberships/activity/:id/members/:sess_cde` Get the number of members (excluding followers) of an activity with activity code `id` in session `:sess_cde`.

`api/memberships/student/:id` Get the memberships of the student with student id `id`.

`api/memberships/student/username:username` Get the public version of memberships of the student with student username `username`.

`api/memberships/isgroupadmin/:id` Get whether or not a student with id `id` is a Group Admin for some activity. Service method is used for security purposes but Controller is currently just for testing convenience.

##### POST

`api/memberships` Create a new membership.

##### PUT

`api/memberships/:id` Edit the membership with membership id `id`.

`api/memberships/:id/group-admin` Toggle whether or not a given member is in a group admin role for a given activity. The `id` parameter is the membership id.

`api/memberships/:id/privacy/:p` Update a given membership to private or not private with boolean value `p`. The `id` parameter is the membership id.

##### DELETE

`api/memberships/:id` Delete the membership with membership id `id`.

### Membership Requests

What is it? Resource that represents a person's application/request to join an activity group.

##### GET

`api/requests` Get all the membership applications.

`api/requests/:id` Get the membership application with request id `id`.

`api/requests/student/:id` Get all the membership applications for the student with student id `id`.

`api/requests/activity/:id` Get all the membership applications for the club with activity code `id`.

##### PUT

`api/requests/:id` Edits an existing memberships application.

##### POST

`api/requests` Create a new membership application.

`api/requests/:id/deny` Deny the membership application with request id `id`.

`api/requests/:id/approve` Approve the membership application with request id `id`.

##### DELETE

`api/requests/:id` Delete the membership application with id `id`.

### Sessions

What is it? Resource that represents the current session. e.g. Fall 2014-2015.

Who has access? Everyone.

##### GET

`api/sessions` Get all the sessions.

`api/sessions/:id` Get the session with session code `id`.

`api/sessions/current` Get the current session.

`api/sessions/daysLeft` Get the days left in the semester and the total days in the semester

### Participation Definitions

What is it? Resource that represents the different levels with which a person can affiliate themselves with a club.

Who has access? Everyone.

##### GET

`api/participations` Get all the possible participation levels.

`api/participations/:id` Get the participation level with code `id`.

`api/participations/leaders` Get the participation levels that are considered leaders.

`api/participations/transcript-worthy` Get the participation levels that should appear on the cct as leadership. Unfinished: TO DO.

### Profiles

What is it? Resource that represents users' profiles.

Differences from GoSite:

-   Only displaying city and country as home address. (When the viewer is a student. Police, super admin, faculty and staff should still see all the information for home address)
-   Displaying minors.
-   On campus was changed to display more general information rather than completely getting rid of it like GoSite does now. (Shows on/off campus)

##### GET

`api/profiles` Get profile info of the current logged in user.

`api/profiles/:username` Get public profile info of a user with username `username` as a parameter.

`api/profiles/role/:username` Get college role of a user with username `username` as a parameter --- College roles: super admin, faculty and staff, student and police.

`api/profiles/Advisors/:username` Get advisor(s) info of a user with username `username` as a parameter.

`api/profiles/clifton/:username` Get the Clifton Strengths of a user with username `username` as a parameter.

`api/profiles/Image/` Get profile image of the current logged in user. Image is stored in a base 64 string.

`api/profiles/Image/:username` Get the profile image(s) of a user with username `username` as a parameter. Image is stored in a base 64 string. Police, super admin, faculty and staff can view both default and preferred profile image of students. Only police and super admin can view both images of everyone including faculty and staff.

##### POST

`api/profiles/image` Upload a preferred image for the current logged in user.

`api/profiles/IDimage` Submit an ID image for the current logged in user.

`api/profiles/image/reset` Delete preferred image and set profile image to default for the current logged in user.

`api/profiles/:type` Update a social media link of a type(facebook, twitter, linkedin,instagram, handshake) of current logged in user.

##### PUT

`api/profiles/mobile_phone_number/:value` Update mobile phone number for the current logged in user.

`api/profiles/mobile_privacy/:value` Update mobile phone number privacy with value(Y or N) for the current logged in user.

`api/profiles/image_privacy/:value` Update profile image privacy with value(Y or N) for the current logged in user.

### Dining

What is it? Request meal plan info and current balances.

Who has access? Everyone.

##### GET

`api/dining` Get all possible meal plan info.

-   If user has one or more meal plans, then current balances for each plan are included in a JSON response.
-   If user does not have a meal plan but has a faculty-staff dining balance, then the balance is returned as a string.
-   If there is no plan or balance then the string "0" (equivalent to no balance) is returned.

### Student Employment

What is it? A resource that represents the campus employments of the currently logged in user.

##### GET

`api/studentemployment` Get the record of campus employments for the currently logged in user.

### Schedule

What is it? Resource that represents a course schedule of user.

##### GET

`api/schedule` Get all schedule objects of the currently logged in user.

`api/schedule/:username` Get all schedule objects of a user with username `username` as a parameter.

### My Schedule

What is it? Resource that represents a customized schedule of user.

##### GET

`api/myschedule` Get all custom events of the currently logged in user.

`api/myschedule/:username` Get all custom events of a user with username `username` as a parameter.

`api/myschedule/event/:eventId` Get a specific custom event of the currently logged in user with `eventId` as a parameter

##### PUT

`api/myschedule/` Update a custom event of the currently logged in user.

##### POST

`api/myschedule/` Create a custom event of the currently logged in user.

##### DELETE

`api/myschedule/:eventID` Delete a custom event of the currently logged in user.

### Schedule Control

What is it? Resource that represents information related to schedule.

##### GET

`api/schedulecontrol` Get the schedulecontrol object of the currently logged in user. Specifically, get the privacy, time last updated, description, and Gordon ID of the currently logged in user's schedule.

`api/schedulecontrol/:username` Get the schedulecontrol object of a user with username `username` as a parameter. Specifically, Get the privacy, time last updated, description, and Gordon ID of the user's schedule.

##### PUT

`api/schedulecontrol/privacy/:value` Update a schedule privacy of the currently logged in user.

`api/schedulecontrol/description/:value` Update a schedule description of the currently logged in user.

### Victory Promise

What is it? Resource that represents the user's scores on the four pillars of the victory promise.

##### GET

`api/vpscore` Get the victory promise scores of the currently logged in user.

### News

What is it? Resource that represents accepted student news entries and news categories.

##### GET

`api/news/category` Gets the full list of category names used to categorize student news as well as category ids and sort order.

`api/news/not-expired` Gets every student news entry that has been accepted and has not yet been in the database 2 weeks or, if the poster set a specific date of expiration, has an expiration date later than the current day.

`api/news/new` Gets every student news entry that has been accepted, has not expired (as described above), and is new since 10am on the day before.

`api/news/personal-unapproved` Gets all of the unexpired and unapproved news submissions by the authenticated user
_(uses stored procedure)_

##### POST

`api/news` Submits a news item into the database (initally unapproved)
_(uses stored procedure)_

##### DELETE

`api/news/:id` Deletes a news item from the database by its id (SNID = student news id). In order to delete, the following conditions must be met:

-   news item is unexpired
-   user is author of news item or SUPER_ADMIN (perhaps should be changed in future)

_(uses repository)_

##### PUT

`api/news/:id` Edits a news item in the database by its id. In order to edit, the following conditions must be met:

-   news item is unexpired
-   user is author of news item or SUPER_ADMIN (perhaps should be changed in future)
-   news item has not yet been approved

_(uses repository)_

### Wellness Check

Back endpoint responsible for fetching and sending information to the database regarding the answers to the wellness check.

##### GET

`api/wellness` Gets the latest answer a student has sent, as well as a boolean that specifies whether the answer is still valid based on when the answer was submitted.

`api/wellness/Question` Gets the wellness check question to be displayed on the front end from the Data base.

##### POST

`api/wellness` Sends an answer boolean to the database that specifies whether a student is symptomatic or not: true = symptomatic, false = not symptomatic.

## API Testing

### Testing Introduction

A test suite is available at `Tests/ApiEndpoints` to exercise the different endpoints. The most important files here are:

-   `test_gordon360_pytest` -- Stores all the necessary variables and authentication.
    -   `activity_code` -- The activity that will be used for testing.
    -   `random_id_number` -- A random id number that is used when we want to verify if we can do things on behalf of someone else. E.g. An advisor can create memberships for anyone. A regular member can only create a membership for him/herself.
    -   `leadership_positions` -- A list of participation levels considered to be leadership positions.
    -   `hostURL` -- Base url of the api
    -   `username` -- String with the username of a test account that is a member of `activity_code`
    -   `password` -- String with the password of a test account that is a member of `activity_code`
    -   `id_number` -- Integer with the id number of the `username`.
    -   `username_activity_leader` -- String with the username of a test account that is a leader of `activity_code`
-   `test_allaccount_pytest` -- Tests for api/accounts/ endpoint
-   `test_allactivities_pytest` -- Tests for api/activities/ endpoint
-   `test_alladmin_pytest` -- Tests for api/admins/ endpoint
-   `test_allauthentication_pytest` -- Tests for token/ endpoint
-   `test_alldining_pytest` -- Tests for api/dining/ endpoint
-   `test_allemail_pytest` -- Tests for api/emails/activity/ endpoint
-   `test_allevents_pytest` -- Tests for api/events/ endpoint
-   `test_allhousingapp_pytest` -- Tests for api/housing/ endpoint
-   `test_allmembership_pytest` -- Tests for api/memberships/ endpoint
-   `test_allmembershiprequest_pytest` -- Tests for api/requests/ endpoint
-   `test_allmyschedule_pytest` -- Tests for api/myschedule/ endpoint
-   `test_allnews_pytest` -- Tests for api/news/ endpoint
-   `test_allprofile_pytest` -- Tests for api/profiles/ endpoint
-   `test_allschedule_pytest` -- Tests for api/schedule/:username/ endpoint
-   `test_allschedulecontrol_pytest` -- Tests for api/schedulecontrol/ endpoint
-   `test_allsession_pytest` -- Tests for api/sessions/ endpoint
-   `test_allstudentemployment_pytest` -- Tests for api/studentemployment/ endpoint
-   `test_allvictorypromise_pytest` -- Tests for api/vpscore/ endpoint
-   `test_allwellnesscheck_pytest` -- Tests for api/wellness/ endpoint

### Running the Tests

Clone the project from the github site:
`git clone https://github.com/gordon-cs/gordon-360-api.git`

Copy credentials.py into gordon-360-api/Tests/ApiEndpoints/. The file is located on the CS-RDSH-02 virtual machine in `C:\Users\Public\Public Documents\` (or `/c/users/public/documents\` when in git-bash). It is the same location where secrets.config is located.

Navigate to the API Tests folder:
`cd gordon-360-api/Tests/ApiEndpoints/`

Install pytest: `pip install -U pytest`

_Note: If you encounter an error of a missing requests import, you may need to install it with `pip install requests` (Summer 2020 fix)_

Check the `hostURL` in test_gordon360_pytest.py if it is pointing to the correct backend

Run the tests:
`pytest` -- This runs all the tests.
`pytest '{name of test file}'` -- This runs a specific test file based on {name of test file}.
`pytest '{name of test file}' -k '{name of def}'` -- This runs a specific test in a specific file based on {name of test file} and {name of def}.

### Writing the Tests

To ensure fewer assertion errors due to future value changes, asserts should avoid values that change frequently over time unless other tests change the values accordingly or the test is set on a specific time.

Suggestions to make tests more robust:

-   Check to see that the response is a list
-   Assert that one of the dictionaries in the list has a certain key
-   Check that the list isn't empty before checking for its content

<span id="manual-test"><!--anchor--></span>

#### Manually Testing the API

To manually test the API, use an API development/testing app like [Postman](https://www.getpostman.com/).

-   Here you can create HTTP requests to hit the API endpoints that you want to test, and see what data response you get back.
-   _Before you can call any normal API endpoints_, you must first call the authentication endpoint with a PUT request, which will give you a token. After starting Postman, use the following steps:

    -   Near the top of the workspace window, change the request type from "GET" to "PUT" using the drop-down menu
    -   Enter the local back-end URL with `/token` appended. It should look something like `http://localhost:5555/token`, but use the port number you selected when carrying out the steps under [Running the server locally](#running-the-server-locally) rather than 5555
    -   Just below the URL you entered, click on "Body"
    -   Check the radio-button for `x-www-form-urlencoded`
    -   Enter the following three pairs in the Key/Value fields, replacing _username_ and _password_ with valid data:

        | Key          | Value      |
        | ------------ | ---------- |
        | `grant_type` | `password` |
        | `username`   | _username_ |
        | `password`   | _password_ |

    -   Click the blue "Send" button - after a brief pause you should see the returned token appear.

-   You can use this token to make an API request. For example:

    -   Use the clipboard to make of copy of the _access-token_ value (do not include the double quotes, just copy the long string of characters between the quotes)
    -   Click on the "+" tab near the top of the window to open a new request frame
    -   Leave "GET" as the request type and enter in an appropriate API URL (e.g. `http://localhost:5555/api/memberships/activity/AJG`
    -   Just below the URL, click on "Headers" and enter the following key/value pairs replacing _access-token_ with the token string you copied:

        | Key             | Value                               |
        | --------------- | ----------------------------------- |
        | `Content-Type`  | `application/x-www-form-urlencoded` |
        | `Authorization` | `Bearer ` _access-token_            |

    -   Click the blue "Send" button

## Troubleshooting

#### Assertion Errors

Some values are dependant on other tests being run after the failed test. To fix this, run the all test files using `pytest` on the command line twice.

Some values are updated frequently over time, to ensure compatibility of tests in the future, value assertions are discouraged. To fix issues from value changes, manually updating or removing the assertions are necessary.

#### 500 Server Error when updating Activity Images

This is usually a folder permissions problem. The Json site runs as the user cct.service@gordon.edu. To solve this issue, edit the permissions to allow the cct.service@gordon.edu user to edit the `browseable` folder. The folder is located in the Api site folder (either 360Api or 360ApiTrain, depending on which is having the problem).

Note that the permissions are reset every time a new `browseable` folder is created. This should not usually happen because the deployment scripts don't touch the `browseable` folder. However, in the case that you delete the old `browseable` folder and put a new one in, make sure to also edit the permissions.

#### 500 Server errors appear all of a sudden, even when nothing has changed in the code base.

At this point, I think we eliminated most c#-related problems. Potential c#-related errors will be throwing custom exceptions that will tell you more about what is wrong. If you do get plain 500 error though, the problem might be a database one.
Check:

-   That the ACT_INFO and ACT_CLUB_DEF tables are in sync.
-   That the stored procedures return exactly what the models expect.
-   That the views are up. Sometimes CTS unexpectedly does maintenance. Try running simple select statements against the Views.

#### 500 Internal Server Error in pytest

500 Internal Server Error could occur anywhere while running pytest. Be careful with all the test inputs, a too long comment might cause 500 error as well. So, try be as neat and clean as possible.

#### 404 Not Found when trying to access the `/token` endpoint:

This error will only pop up when you are testing the server directly by running it with visual studio. When you run the gordon360 server from visual studio it automatically "hosts" it on `localhost:3000`. By default, the server doesn't accept non-HTTPS (anything not on port 443) connections. There are two solutions:

-   Change Visual studio settings to run on `localhost:443` by default. I tried this a bit, but didn't get very far. I don't think it is that hard though, I just didn't have the motivation to continue.
-   Allow non-HTTPS connections by commenting out some code. This is what I did. DON'T FORGET TO REMOVE THE COMMENT SYMBOLS AFTER YOU FINISH THOUGH.
    -   The code that restricts non-HTTPS connections is located under the `Startup.cs` file. Look for the "#if DEBUG" and "#endif" code-blocks. Comment both out.

#### 415 Unsupported Media Error

This error might occurs while testing 'test_post_profile_image' and 'test_post_ID_image', the reason causes this is that method 'PostAsFormData()' doesn't post it as form data, it is still JSON, fix should be happened in pytest_components.py. However, even if 415 error was fixed, it would still possibly cause error 500, potentail solutions are above.

### Documentation

The documentation folder currently contains the ColdFusion files from go.gordon that contain the logic behind the people search.

-   Index.cfm is the page used to select search criteria.
-   Searchresults.cfm is the list of people you get back based on that criteria. It selects from a student view, facstaff view, and alumni view all separately and then sorts all the results together.
-   Showperson.cfm is the detail page of the person you select from the searchresults.cfm page.

Team members: Bradley Boutcher, Chris Qiao, Jenny Kim, Joe Ross, Matt Felgate, Sam Nguyen

Computer Science Summer Practicum 2017
