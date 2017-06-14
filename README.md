# Gordon 360

#### The API consumed by [Project Bernard](https://github.com/gordon-cs/Project-Bernard)
Dive in.
## Table of Contents
- [Machines and Sites](#machines-and-sites)
    - [Deploying to the Api Site](#deploying-to-the-api-site)
    - [Deploying to the Front-end site](#deploying-to-the-front-end-site)
- [The Database](#the-database)
    - [Tables](#tables)
    - [Stored Procedures](#stored-procedures)
    - [Triggers](#triggers)
- [The Code](#the-code)	
    - [Introduction](#introduction) 
- [API Endpoints](#api-endpoints)
    - [Authentication](#authentication)
    - [Memberships](#memberships)
    - [Activities](#activities)
    - [Membership Requests](#membership-requests)
    - [Students](#students)
    - [Accounts](#accounts)
    - [Sessions](#sessions)
    - [Participation Definitions](#participation-definitions)
    - [Emails](#emails)
    - [Admins](#admins)
    - [Content Management](#content-management)
- [API Testing](#api-testing)
    - [Introduction](#introduction)
    - [Running the Tests](#running-the-tests)
    - [Running the server locally](#running-the-server-locally)
- [Troubleshooting](#troubleshooting)

## Machines and Sites
To work on this project, it is easiest to use the following machines provided by CTS:
- CCCTrain.gordon.edu - Windows machine.
    - Can be accessed through Remote Desktop Connection.
    - Has the C# code.
    - Has Visual Studio, MSSQL Server.
    - Has deployment scripts and folders.
    - Has site folders.
- CS-360-API-TEST.gordon.edu - Ubuntu machine
    - Is accessed through ssh.
    - Is setup for running the tests (has an already filled `test_credentials.py` file.)
    - Has the User-facing code (HTML, JS and CSS)


The folders for these IIS sites can be found on the CCCTrain machine under `F:\sites`. 
- 360.gordon.edu -- Production Front-end. User-facing code (css, js, html)
- 360Train.gordon.edu -- Development Front-end. User-facing code (css, js, html)
- 360Api.gordon.edu -- Production JSON server site. C# using the ASP.NET Framework.
- 360ApiTrain.gordon.edu -- Development JSON server site. C# using the ASP.NET Framework.

### Deploying to the Api Site

- Log in to CCCTrain and start Visual Studio as the cct.service user. (Shift + right click)
- Open an existing project/solution - `C:\users\cct.service\code\Project-Raymod\Gordon360` file. It is a Microsoft Visual Studio Solution file.
- Make a change. Do your thing.
- Menu Bar -> Build - Publish Gordon360.
- Choose the right publish profile.  
    - DEV -- Development ( Connects to the admintrainsql database server, and used for 360train.gordon.edu). 
    - Prod -- Production ( Connects to the adminprodsql database server, and used for the real site 360.gordon.edu).
- Clicking publish pushes your changes to the API for either 360ApiTrain.gordon.edu or 360Api.gordon.edu, depending on which publish profile you used.

### Deploying to the Front-end Site

- Log into CS-360-API-TEST through ssh.
- To make a change to the code, clone the [Project Bernard](https://github.com/gordon-cs/Project-Bernard) repository.
- Install EmberJS and its dependencies. See the Project Bernard repository for help on how to do this (If you are using the CS-360-API-TEST machine, skip this step).
- Make a change to the code. Do your thing, make your mark. A legacy.
- Run one of these commands in the terminal at the root of the project folder.
    - `ember build --env development` -- This version will use the Development api endpoint (360ApiTrain.gordon.edu)
    - `ember build --env production` -- This version will use the Production api endpoint (360Api.gordon.edu)
- The output is placed in the `dist/` folder at root of your project folder.
    - Note: Since emberJS is a javascript framework, the output is just an html file with a TON of javascript linked :p
- Move the `dist/` folder AS-IS to one of the two user-facing sites on CS-RDP1.
    - If you used the development flag, move `dist/` to the 360Train IIS site.
    - If you used the production flag, move `dist/` to the 360 IIS site.
- For moving files between a mac and the virtual windows machine, we used a Microsoft Remote Desktop feature called folder redirection. It lets you specify folders on your mac that will be available on the PC you are remoting to.


## The Database

The `CCT` database exists in:
- `admintrainsql.gordon.edu` - The development/test database server
- `adminprodsql.gordon.edu` -  The production/live database server.

### Tables

All the tables were created from scratch by our team. 

Misc Information:
- Apart from a few exceptions, the tables don't make use of foreign key constraints. This is because the relevant primary keys are in the tables referenced by Views. Unfortunately, one cannot make foreign keys that reference Views. 


###### ACT_INFO

A record in this table stores:
- ACT_CDE - The short code for the activity.
- ACT_DESC - The name of the activity.
- ACT_BLURB - A short description of what the activity is about. This will be filled out by a leader.
- ACT_URL - URL to the website for the club/organization (if they have one).
- ACT_IMAGE_PATH - Path to where the activity logo is stored in the browseable folder.
- ACT_TYPE - Short code for the type of the activity
- ACT_TYPE_DESC - Full name of the type of the activity

You might notice that this table is an extension of the ACT_CLUB_DEF view. It contains extra information that the view does not have, but that we need. This is clearly a case of Information Duplication; information is available in two places and can easily fall out of sync. To remedy this, the stored procedure [UPDATE_ACT_INFO](#update_act_info) was made.

###### ADMIN

A record in this table stores:

- ID_NUM - The gordon id number of the administrator
- USER_NAME - The administrator's username
- EMAIL - The administrator's email.
- SUPER_ADMIN - Whether or not the admin has super admin privilege

To make someone an admin, simply insert a record into this table through MSSQL Studio. 

###### JNZB_ACTIVITIES

A record in this table stores all the same fields as an Activity table in Jensibar would. 
The goal of this table was to contain membership information that was to be moved to Jenzibar. To do this, one would use the stored procedure [UPDATE_JNZB_ACTIVITIES](#update_jnzb_activities).

###### MEMBERSHIP

A record in this table stores:

- ACT_CDE - The short code for the activity.
- SESS_CDE - The short code for the session when this membership took place.
- ID_NUM - The gordon id number of the member.
- PART_CDE - The short code for the participation level of the member.
- BEGIN_DTE - The date the membership began
- END_DTE - The date the membership ended
- COMMENT_TXT - Comment about the membership
- GRP_ADMIN - A boolean indicating whether or not this member has group admin privileges, allowing them to edit the group's page on the site

The other three fields (USER_NAME, JOB_NAME and JOB_TIME) where meant to be administrative fields to store data about who inserted records and when they did it. We ended up not using them. We kept them because they have good potential use.

###### REQUEST

A record in this table stores:

- SESS_CDE - The short code for the session they are requesting to join.
- ACT_CDE - The short code of the activity they are requesting to join.
- ID_NUM - The gordon id number of the potential member.
- PART_CDE - The short code for the participation level they want to join as.
- DATE_SENT - The date the request was made.
- COMMENT_TXT - Comments to accompany the request.
- STATUS - Status of the request. Should be either Pending, Approved or Denied.\


###### ACT_CLUB_DEF

A record in this table stores

- ACT_CDE - The activity short code.
- ACT_DESC - The name of the activity.

This table is an exact duplicate of the JENZ_ACT_CLUB_DEF view. It is periodically updated by making sure what is in it corresponds to what is in JENZ_ACT_CLUB_DEF. When a new activity is found in JENZ_ACT_CLUB_DEF, it is inserted into ACT_CLUB_DEF and the stored procedure UPDATE_ACT_INFO is run.


### Views

We got access to these views through CTS. They are a direct live feed from the tables they represent. As mentioned earlier, we cannot use primary keys in the views to make foreign keys in other tables.  

###### ACCOUNT
Account information for all the members of gordon college.
###### JENZ_ACT_CLUB_DEF
The Activity information. Includes short codes and what they represent.
###### CM_SESSION_MSTR
The Session information. Includes short codes, the session they represent, and the physical dates spanned by the session.
###### Faculty
A subset of `ACCOUNT` that has only faculty member records.
###### PART_DEF
Definitions of the different participation levels for someone in an activity.
###### Staff
A subset of `ACCOUNT` that has only staff member records.
###### Student
A subset of `ACCOUNT` that has only student records.
###### 360_SLIDER
Content (images, captions, and links) for the slider on the dashboard page.

### Stored Procedures

Stored procedures have been written to make some database accesses and administrative tasks easier.
Here are the most important ones.

###### UPDATE_ACT_CLUB_DEF

This keeps the ACT_CLUB_DEF table in sync with the JENZ_ACT_CLUB_DEF view. It should be run periodically.

###### UPDATE_ACT_INFO

Because ACT_INFO is basically a duplicate of ACT_CLUB_DEF, this stored procedure tries to keep them synced. Ideally it should be run automatically anytime ACT_CLUB_DEF changes.

In non-sql terms, this procedure makes sure all the activities defined in ACT_CLUB_DEF are also present in ACT_INFO. If something has been added/removed to ACT_CLUB_DEF but not in ACT_INFO, it adds/removes the corresponding record to ACT_INFO.


###### UPDATE_JNZB_ACTIVITIES

This stored procedures is pretty simple. It moves all the relevant information from the MEMBERSHIP table and puts it in the JNZB_ACTIVITIES table. To prevent duplication, it will only add records that are present in the MEMBERSHIP table, but missing the JNZB_ACTIVITIES table.


### Triggers

###### ACT_CLUB_DEF_INSERT_TRIGGER

Everytime a record is inserted into the ACT_CLUB_DEF table, this trigger runs the UPDATE_ACT_INFO stored procedure. Although not clear in the name, this trigger also runs whenever a row is deleted from ACT_CLUB_DEF. 

## The Code

### Introduction

The server was written using ASP.NET and is generally structured as such. As a MVC (Model View Controller) system, the heart of the code is in ApiControllers (which is organized like the API it implements, which is documented later in this file) and in the Models folder. The View is provided by a separate repository, Project-Bernard.

Here is a breakdown of the project folder:

- Project-Raymond/
    - Design_Documents/ - currently empty. I do not actually remember why we had this. 
    - Gordon360/ - The main project. Most of the work will be done here.
        - ApiControllers/ - Folder contatining the Controllers for the API endpoints.
        - AuthorizationFilters/ - Contains code that enforces rules about who can access what.
        - AuthorizationServer/ (The folder should really be called AuthenticationServer) -  Contains code that does user authentication.
        - bin/ - binary files. nothing to see here.
        - browseable/ - Placeholder folder that will be moved AS-IS to the built product. It will end up containing user-generated content. Right now, it only contains uploaded activity pictures.
        - Documentation/ - All the comments in the code are concatenated and made into this file. This is an automatic ASP.NET feature. We were going to somehow use the generated file for documentation, but didn't go through with it.
        - Exceptions/ - Custom exceptions that are thrown in the code. These exceptions get thrown instead of the default 500 Server Error Exception.
        - Models/ Code for the models (Model as in MODEL-View-Controller, or MVC)
        - obj/ - Not really sure what this is. It is also automatically generated by ASP.NET.
        - Properties/ - Contains files that Visual Studio uses to build and publish the project. No need to dig into this unless you want to fine tune the build process.
        - Repositories/ - Contains Repository files and Unit of Work Files. Both of these are object-oriented design patterns. They help with seperation of concerns and general code organization.
        - Services/ - Services that are used by the ApiController. The concept of a Service is also a design pattern. It is very useful for decoupling. (e.g. Making sure Controller code is seperate from code that accesses the database).
        - Static Classes/ - Helper classes that are used throughout the code. 
        - Stored Procedures/ - This folder can be deleted. It is a relic from the past when I used to hard-code my stored procedures.
    - packages/ - ASP.NET packages that the project depends on. You will not be making any changes here.
    - Tests/ - Folder for tests
        - ApiEndpoints/ - I talk about this more in the Testing section. 

## API Endpoints

### Authentication

##### POST

`/token:` The authentication process uses a simplified version of what is called Open Authentication (OAuth).

In OAuth, there are two servers including the one running your app. The server running your app doesn't authenticate people directly, it relies on the second server to tell it if a given person is allowed access. This second server is called the Authentication Server. 

In our project, the Authentication Server and the App Server are actually the same. They are only seperated code-wise. You could say that the *App* is the ApiControllers folder and the *Authentication Server* is the AuthorizationServer folder (Recall that it is currently named incorrectly, sorry. We haven't changed the name because it would require changing multiple import statements, and we don't have time to debug after the change).

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


### Memberships
What is it? Resource that respresents the affiliation between a student and a club.

##### GET

`api/memberships` Get all the memberships.

`api/memberships/:id` Get the membership with membership id `id`.

`api/memberships/activity/:id` Get the memberships associated with the activity with activity code `id`.

`api/memberships/activity/:id/leaders` Get the memberships of the leaders for the activity with activity code `id`.

`api/memberships/activity/:id/advisors` Get the memberships of the adviors for the activity with activity code `id`.

`api/memberships/activity/:id/group-admin` Get the memberships of the group admin (displayed as "Group Contacts") for the activity with activity code `id`.

`api/memberships/student/:id` Get the memberships of the student with student id `id`.

##### POST

`api/memberships` Create a new membership.

##### PUT 

`api/memberships/:id` Edit the membership with membership id `id`.

`api/memberships/:id/group-admin` Toggle whether or not a given member is in a group admin role for a given activity. The `id` parameter is the membership id.

##### DELETE

`api/memberships/:id` Delete the membership with membership id `id`.


### Activities
What is it? Resource that represents some activity - such as a club, ministry, leadership program, etc.

##### GET

`api/activities` Get all the activities.

`api/activities/:id` Get the activity with activity code `id`.

`api/activities/session/:id` Get the activity offered during the session with session code `id`.

`api/activities/session/:id/types` Get the different activity types among the activities offered during the session with session code `id`.

`api/activities/{sessionCode}/{id}/status` Get the status of an activity (either open or closed), which indicates whether or not new members can be added to the activity for this session.

`api/activities/open` Get all the open activities for the current session.

`api/activities/:id/open` Get only the open activities for which a given user (identified by their user `id`) is the group admin.

`api/activities/closed` Get all the closed activities for the current session.

`api/activities/:id/closed` Get only the closed activities for which a given user (identified by their user `id`) is the group admin.

`api/activities/:id/session/{sess_cde}/close` Close out an activity for a given session (this is like confirming the final roster of an activity for a given session.

`api/activities/:id/session/{sess_cde}/open` Reopen an activity for a given session.


##### PUT

`api/activities/:id` Edit activity information for the club with activity code `id`.

### Membership Requests
What is it? Resource that represents a person's application/request to join an activity group.

##### GET

`api/requests` Get all the membership applications.

`api/requests/:id` Get the membership application with request id `id`.

`api/requests/student/:id` Get all the membership applications for the student with student `id`.

`api/requests/activity/:id` Get all the membership applications for the club with activity code `id`.


##### POST

`api/requests` Create a new membership application.

`api/requests/:id/deny` Deny the membership application with request id `id`.

`api/requests/:id/approve` Approve the membership application with request id `id`.

##### DELETE

`api/requests/:id` Delete the membership application with id `id`.


### Students
What is it? Resource that represents a student.

##### GET

`api/students` Get all the students.

`api/students/:id` Get the student with student id `id`.

`api/student/:email` Get the student with email `email`.


### Accounts
What is it? Resource that represents a gordon account.

##### GET

`api/accounts/:email` Get the account with email `email`.

### Sessions
What is it? Resource that represents the current session. e.g. Fall 2014-2015.

Who has access? Everyone.

##### GET

`api/sessions` Get all the sessions.

`api/sessions/:id` Get the session with session code `id`.

`api/sessions/current` Get the current session.

### Participation Definitions
What is it? Resource that represents the different levels with which a person can affiliate themselves with a club.

Who has access? Everyone.

##### GET

`api/participations` Get all the possible participation levels.

`api/partipations/:id` Get the participation level with code `id`.


### Emails
What is it? Resource that represents emails. 


##### GET 

`api/emails/activity/:id` Get the emails for members of the activity with activity code `id` during the current session.

`api/emails/activity/:id/session/:sessionid` Get the emails for the members of the activity with activity code `id` during the session with session code `sessionid`.

`api/emails/activity/:id/leaders` Get the emails for the leaders of the activity with activity code `id` during the current session.

`api/emails/activity/:id/leaders/session/:sessionid` Get the emails for the leaders of the activity with activity code `id` during the session with session code `sessionid`.

`api/emails/activity/:id/advisors` Get the emails for the advisors of the activity with activity code `id` during the current session.

`api/emails/activity/:id/advisors/session/:sessionid` Get the emails for the advisors of the activity with activity code `id` during the session with session code `sessionid`.


### Admins
What is it? Ressource that represents admins.

Who has access? Only super admins, except to get a specific admin where all admins have access.

##### GET

`api/admins` Get all the admins.

`api/admins/:id` Get a specific admin with the Gordon ID specified.

##### POST

`api/admins` Create a new admin.

##### DELETE

`api/admins/:id` Delete the admin with the admin id `id`.


### Content Management
What is it? Resource for fetching content that has been stored in the database by Gordon's website [content manager](http://wwwtrain.gordon.edu/).

##### GET

`api/cms/slider` Get the content for the dashboard slide.


## API Testing

### Introduction 

A test suite is available at `Tests/ApiEndpoints` to excercise the different endpoints. The most important files here are:
- `gordon_360_tests_leader.py` -- Tests the api endpoints while authorized as an activity leader.
- `gordon_360_tests_member.py` -- Tests the api endpoints while authorized as a regular member.
- `test_config.py` -- Configuration options, includes the following variables:
    - `activity_code` -- The activity that will be used for testing. Tests under `gordon_360_tests_leader.py` assume the account used for testing is a leader of this activity. Tests under `gordon_360_tests_member.py` assume the account used for testing is a member of this activity.
    - `random_id_number` -- A random id number that is used when we want to verify if we can do things on behalf of someone else. E.g. An advisor can create memberships for anyone. A regular member can only create a membership for him/herself.
    - `leadership_positions` -- A list of participation levels considered to be leadership positsions.
    - `hostURL` -- Base url of the api
- `test_credentials.py` -- (If you cloned the project, you need to create this file) File with credentials the test program will use.
	- `username` -- String with the username of a test account that is a member of `activity_code` in `test_config.py`.
	- `password` -- String with the password of a test account that is a member of `activity_code` in `test_config.py`.
	- `id_number` -- Integer with the id number of the `username`.
	- `username_activity_leader` -- String with the username of a test account that is a leader of `activity_code` in `test_config.py`.
	- `password_activity_leader` -- String with the password of a test account that is a leader of `activity_code` in `test_config.py`.
	- `id_number_activity_leader` -- Integer with the id number of the `username_leader`.

### Running the Tests

Clone the project from the github site:
`git clone https://github.com/gordon-cs/Project-Raymond.git`

Navigate to the API Tests folder:
`cd Project-Raymond/Tests/ApiEndpoints/`

Create the `test_credentials.py` file and define the six variables mentioned above.
Make sure the credentials you enter match the descriptions provided above.

Verify that the variables defined in `test_config.py` are correct.

Run the tests:
`python3 gordon_360_tests.py` -- This runs all the tests. For both members and leaders.
`python3 gordon_360_tests_member.py` -- This runs the tests for members.
`python3 gordon_360_tests_leader.py` -- This runs the tests for leaders.

### Manual Testing

##### Running the server locally

* Before you begin you will have to add the secrets.config file to the folder that you are working from. The file is located on the cs-devA virtual machine in `C:\Users\Public\Documents\360 Shared files` Copy the file secrets.config to the same folder in your project that contains the web.config file. This will allow you to run the server locally.

* If you are using the virtual machine you will need to run the server on an unused port.  To change the port that the server is running open the solution in virtual studio.  In the solution explorer, right click the name of the project (Gordon360) and select properties.  Choose the Web tab and change the Project Url to an unused port.

* You can then press the Start button in virtual studio to run the server. It will open the web browser and display an Error 403.14 - Forbdden. This is expected.  You can now begin manually testing the API

To manually test the API, use an API development/testing app like [Postman](https://www.getpostman.com/).
* Here you can create HTTP requests to hit the API endpoints that you want to test, and see what data response you get back. 
* _Before you can call any normal API endpoints_, you must first call the authentication endpoint, which will give you a token.
	* E.g. Call `localhost:3000/token` with the following (key, value) pairs in the request body: (username, _MYUSERNAME_), (grant_type, password), (password, _MYPASSWORD_). This will give me back a long token string. I then can copy that token and paste it in the Authorization header of another API request I want to make. 
* Making a normal API request: 
	* E.g. Call something like `localhost:3000/api/memberships/activity/AJG` and under Headers you will need two (as key, value pairs):
		1. (`Content-Type`, `application/x-www-form-urlencoded`) - usually this is the content type. Subject to change, though.
		2. (`Authorization`, `Bearer  [MYTOKEN]`)


Team members: Eze Anyanwu, James Kempf, Adam Bartholomew

Computer Science Summer Practicum 2016

## Troubleshooting

#### 500 Server Error when updating Activity Images

This is usually a folder permissions problem. The Json site runs as the user cct.service@gordon.edu. To solve this issue, edit the permissions to allow the cct.service@gordon.edu user to edit the `browseable` folder. The folder is located in the Api site folder (either 360Api or 360ApiTrain, depending on which is having the problem).

Note that the permissions are reset everytime a new `browseable` folder is created. This should not usually happen because the deployment scripts don't touch the `browseable` folder. However, in the case that you delete the old `browseable` folder and put a new one in, make sure to also edit the permissions.


#### 500 Server errors appear all of a sudden, even when nothing has changed in the code base.

At this point, I think we eleminiated most c#-related problems. Potential c#-related errors will be throwing custom exceptions that will tell you more about what is wrong. If you do get plain 500 error though, the problem might be a database one.
Check:
- That the ACT_INFO and ACT_CLUB_DEF tables are in sync.
- That the stored procedures return exactly what the models expect.
- That the views are up. Sometimes CTS unexpectedly does maintainance. Try running simple select statements against the Views.

#### 404 Not Found when trying to access the `/token` endpoint:

This error will only pop up when you are testing the server directly by running it with visual studio. When you run the gordon360 server from visual studio it automatically "hosts" it on `localhost:3000`. By default, the server doesn't accept non-HTTPS (anything not on port 443) connections. There are two solutions:
- Change Visual studio settings to run on `localhost:443` by default. I tried this a bit, but didn't get very far. I don't think it is that hard though, I just didn't have the motivation to continue.
- Allow non-HTTPS connections by commenting out some code. This is what I did. DON'T FORGET TO REMMOVE THE COMMENT SYMBOLS AFTER YOU FINISH THOUGH. 
    - The code that restricts non-HTTPS connections is located under the `Startup.cs` file. Look for the "#if DEBUG" and "#endif" code-blocks. Comment both out. 
