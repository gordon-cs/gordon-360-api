# Gordon 360

### (IN PROGRESS...)

Dive in.
## Table of Contents
- [Sites](#sites)
    - [Deploying to the Api Site](#deploying-to-the-api-site)
    - [Deploying to the Front-end site](#deploying-to-the-front-end-site)
- [The Database](#the-database)
    - [Tables](#tables)
    - [Stored Procedures](#stored-procedures)
- [The Code](#the-code)	
    - [Introduction](#introduction) 
- [API Endpoints](#api-endpoints)
    - [Authentication] (#authentication)
    - [Memberships](#memberships)
    - [Clubs](#clubs)
    - [Membership Requests](#membership-requests)
    - [Supervisors](#supervisors)
    - [Students](#students)
    - [Accounts](#accounts)
    - [Sessions](#sessions)
    - [Participation Definitions](#participation-definitions)
    - [Emails](#emails)
- [API Testing](#api-testing)
    - [Setting up](#setting-up)
- [Troubleshooting](#troubleshooting)

## Sites 
The folders for these IIS sites can be found on the CS-RDP1 machine under `F:\sites`. 
- 360.gordon.edu -- Production Front-end. User-facing code (css, js, html)
- 360Train.gordon.edu -- Development Front-end. User-facing code (css, js, html)
- 360Api.gordon.edu -- Production JSON server site. C# using the ASP.NET Framework.
- 360ApiTrain.gordon.edu -- Development JSON server site. C# using the ASP.NET Framework.

### Deploying to the Api Site

- Log in to CS-RDP1 and start Visual Studio as the cct.service user. (Shift + right click)
- Open an existing project/solution - `C:\users\cct.service\code\Project-Raymod\Gordon360` file. It is a Microsoft Visual Studio Solution file.
- Make a change. Do your thing.
- Menu Bar -> Build - Publish Gordon360.
- Choose the right publish profile.  
    - DEV -- Development ( Uses the admintrain connection string). 
    - Prod -- Production ( Uses the adminprod connection string).
- Clicking publish creates a Package. A Package in this context is a basically a zipped-up version of the project.
- The package saves to `C:\users\cct.service\Gordon360Deploy`  and depending on what you published, it will go to the right folder.
    - deploy_production.bat -- moves the package in the ReleasePackage to 360Api
    - deploy_development.bat -- moves the package in the Development Package to 360ApiTrain.
- The scripts need to be run as an admin.

### Deploying to the Front-end Site

- This is easier done on a mac.
- To make a change to the code, clone the [Project Bernard]() repository to your (hopefully) mac.
- Install EmberJS and its dependencies. See the Project Bernard repository for help on how to do this.
- Make a change to the code. Do your thing, make your mark. A legacy.
- Run one of these commands in the terminal at the root of the project folder.
    - `ember build --env development` -- This version will use the Development api endpoint (360ApiTrain.gordon.edu)
    - `ember build --env production` -- This version will use the Production api endpoint (360Api.gordon.edu)
- The output is placed in the `dist/` folder at root of your project folder.
    - Note: Since emberJS is a javascript framework, the output is just an html file with a TON of javascript linked :p
- Move the `dist/` folder as is to one of the sites on CS-RDP1.
    - If you used the development flag move `dist/` to the 360Train IIS site.
    - If you used the production flag, move `dist/` to the 360 IIS site.
- For moving files between a mac and the virtual windows machine, we used a Microsoft Remote Desktop feature called folder redirection. It lets you specify folders on your mac that will be available on the PC you are remoting to.

`API Url: ` Coming soon...

## The Database

The `CCT` database exists in:
- `admintrainsql.gordon.edu` - The Train/dev database server
- `adminprodsql.gordon.edu` -  The production/live database server.

### Tables

All the tables were created from scratch by our team. 

Misc Information:
- Apart from a few exceptions, the tables don't make use of foreign key constraints. This is because the relevant primary keys are in the tables referenced by Views. Unfortunately, one cannot add foreign keys that reference Views. 


###### ACT_INFO

A record in this table stores:
- ACT_CDE - The short code for the activity.
- ACT_DESC - The name of the activity.
- ACT_BLURB - A short description of what the activity is about. This will be filled out by a leader.
- ACT_URL - URL to the website for the club/organization (if they have one).
- ACT_IMAGE_PATH - Path to where the activity logo is stored in the browseable folder.

You might notice that this table is an extension of the ACT_CLUB_DEF view. It contains extra information that the view does not have, but that we need. This is clearly a case of Information Duplication; information is available in two places and can easily fall out of sync. To remedy this, the stored procedure [UPDATE_ACT_INFO](#update_act_info) was made.

###### ADMIN

A record in this table stores:

- ID_NUM - The gordon id number of the administrator
- USER_NAME - The administrator's username
- EMAIL - The administrator's email.

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


###### SUPERVISOR 

A record in this table stores:

- ID_NUM - The gordon id number of the potential member.
- SESS_CDE - The session short code during which this person is a supervisor.
- ACT_CDE - The activity short code for which this person is a supervisor.

The other three fields (USER_NAME, JOB_NAME and JOB_TIME) where meant to be administrative fields to store data about who inserted records and when they did it. We ended up not using them. We kept them because they have good potential use.

### Views

We got access to these views through CTS. They are a direct live feed from the tables they represent. As mentioned earlier, we cannot use primary keys in the views to make foreign keys in other tables.  

###### ACCOUNT
Account information for all the members of gordon college.
###### ACT_CLUB_DEF
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

### Stored Procedures

Stored procedures have been written to make some database accesses and administrative tasks easier.
Here are the most important ones.

###### UPDATE_ACT_INFO

Because ACT_INFO is basically a duplicate of ACT_CLUB_DEF, this stored procedure tries to keep them synced. Ideally it should be run automatically anytime ACT_CLUB_DEF changes. If that is not possible, it should be run periodically (e.g. daily). 
In non-sql terms, this procedure makes sure all the activities defined in ACT_CLUB_DEF are also present in ACT_INFO. If something has been added to ACT_CLUB_DEF but is not present in ACT_INFO, it adds the corresponding record to ACT_INFO, filling in the other columns with default data.

###### UPDATE_JNZB_ACTIVITIES

This stored procedures is pretty simple. It moves all the relevant information from the MEMBERSHIP table and puts it in the JNZB_ACTIVITIES table. To prevent duplication, it will only add records that are present in the MEMBERSHIP table, but missing the JNZB_ACTIVITIES table.

## The Code

### Introduction

The server was written using ASP.NET and is generally structured as such. Here is a breakdown of the project folder:

- Project-Raymond/
    - Design_Documents/ - currently empty. I do not actually remember why we had this. 
    - Gordon360/ - The main project. Most of the work will be done here.
        - ApiControllers/ - Folder contatining the Controllers for the API endpoints.
        - AuthorizationFilters/ - Custom ASP.NET Authorization filters. 
        - AuthorizationServer/ - Code related to Open Authentication (OAuth).
        - bin/ - binary files. nothing to see here.
        - browseable/ - Placeholder folder that will be moved AS-IS to the built product. It will end up containing user-generated content. Right now, it only contains uploaded activity pictures.
        - Documentation/ - All the comments in the code are concatenated and made into this file. This is an automatic ASP.NET feature. We were going to somehow use the generated file for documentation, but didn't go through with it.
        - Exceptions/ - Custom exceptions that are thrown in the code.
        - Models/ Code for the models (Model as in MODEL-View-Controller, or MVC)
        - obj/ - Not really sure what this is. It is also automatically generated by ASP.NET.
        - Properties/ - Contains files that Visual Studio uses to build and publish the project. No need to dig into this unless you want to fine tune the build process.
        - Repositories/ - Contains Repository files and Unit of Work Files. Both of these are design patterns. It might be useful to research more about them.
        - Services/ - Services that are used by the ApiController. The concept of a Service is also a design pattern.
        - Static Classes/ - Helper classes that are used throughout the code. 
        - Stored Procedures/ - This folder can be deleted. It is a relic from the past when I used to hard code my stored procedures into the code.
    - packages/ - ASP.NET packages that the project depends on. You will not be making any changes here.
    - Tests/ - Folder for tests
        - ApiEndpoints/ - I talk about this more in the Testing section. 

## API Endpoints

### Authentication

##### POST

`/token:` Implements Open Authentication (OAuth). 

Accepts a form encoded object in the body of the request: 
```
{ 
	"username": YOUR-USERNAME, 
	"password": YOUR-PASSWORD, 
	"grant_type": "password" 
}
```
Response will include an access token which should be included in subsequent request headers.
Specifically include it in the `Authorization` header like so `Bearer YOUR-ACCESS-TOKEN`


### Memberships
What is it? Resource that respresents the affiliation between a student and a club.

Who has access? It's complicated.

##### GET

`api/memberships` Get all the memberships.

`api/memberships/:id` Get the membership with membership id `id`.

`api/memberships/activity/:id` Get the memberships associated with the activity with activity code `id`.

`api/memberships/activity/:id/leaders` Get the memberships of the leaders for the activity with activity code `id`.

`api/memberships/student/:id` Get the memberships of the student with student id `id`.

##### POST

`api/memberships` Create a new membership.

##### PUT 

`api/memberships/:id` Edit the membership with membership id `id`.

##### DELETE

`api/memberships/:id` Delete the membership with membership id `id`.


### Clubs
What is it? Resource that represents a club.

Who has access? It's complicated.

##### GET

`api/activities` Get all the clubs.

`api/activities/:id` Get the club with activity code `id`.

`api/activities/session/:id` Get the clubs offered during the session with session code `id`.

##### PUT

`api/activities/:id` Edit club information for the club with activity code `id`.

### Membership Requests
What is it? Resource that represents a person's application/request to join a club.

Who has access? It's complicated.

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

### Supervisors
What is it? Resource that represents the supervisor of an activity.

Who has access? It's complicated.

##### GET

`api/supervisors` Get all the supervisors.

`api/supervisors/:id` Get the supervisor with supervisor id `id`.

`api/supervisors/activity/:id` Get the supervisors for the activity with activity code `id`.

##### POST

`api/supervisors` Create a new supervisor.

#### PUT

`api/supervisors/:id` Edit the supervisor with supervisor id `id`.

##### DELETE

`api/supervisors/:id` Delete the supervisor with supervisor id `id`.


### Students
What is it? Resource that represents a student.

Who has access? Probably not you.

##### GET

`api/students` Get all the students.

`api/students/:id` Get the student with student id `id`.

`api/student/:email` Get the student with email `email`.


### Accounts
What is it? Resource that represents a gordon account.

Who has access? Probably not you.

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

Who has access? It's complicated.


##### GET 

`api/emails/activity/:id` Get the emails for members of the activity with activity code `id` during the current session.

`api/emails/activity/:id/session/:sessionid` Get the emails for the members of the activity with activity code `id` during the session with session code `sessionid`.

`api/emails/activity/:id/leaders` Get the emails for the leaders of the activity with activity code `id` during the current session.

`api/emails/activity/:id/leaders/session/:sessionid` Get the emails for the leaders of the activity with activity code `id` during the session with session code `sessionid`.



## API Testing

A test suite is available at `Tests/ApiEndpoints` to excercise the different endpoints. The most important files here are:
- `gordon_360_tests_leader.py` -- Tests the api endpoints while authorized as an activity leader.
- `gordon_360_tests_member.py` -- Tests the api endpoints while authorized as a regular member.
- `test_config.py` -- Configuration options, includes the following variables:
    - `activity_code` -- The activity that will be used for testing. Tests under `gordon_360_tests_leader.py` assume the account used for testing is a leader of this activity. Tests under `gordon_360_tests_member.py` assume the account used for testing is a member of this activity.
    - `random_id_number` -- A random id number that is used when we want to verify if we can do things on behalf of someone else. E.g. A supervisor can create memberships for anyone. A regular member can only create a membership for him/herself.
    - `leadership_positions` -- A list of participation levels considered to be leadership positsions.
    - `hostURL` -- Base url of the api
- `test_credentials.py` -- (If you cloned the project, you need to create this file) File with credentials the test program will use.
	- `username` -- String with the username of a test account that is a member of `activity_code` in `test_config.py`.
	- `password` -- String with the password of a test account that is a member of `activity_code` in `test_config.py`.
	- `id_number` -- Integer with the id number of the `username`.
	- `username_leader` -- String with the username of a test account that is a leader of `activity_code` in `test_config.py`.
	- `password_leader` -- String with the password of a test account that is a leader of `activity_code` in `test_config.py`.
	- `id_number_leader` -- Integer with the id number of the `username_leader`.

### Setting up

The following instructions are for Mac machines.

Clone the project from the github site:
`git clone https://github.com/gordon-cs/Project-Raymond.git`

Navigate to the API Tests folder:
`cd Project-Raymond/Tests/ApiEndpoints/`

Create the `test_credentials.py` file and define the six variables mentioned above.
Make sure the credentials you enter match the descriptions provided above.

Run the tests. The results of the tests are displayed as they are run. If a test fails, the reason for failure will also be displayed.



Team members: Eze Anyanwu, James Kempf, Adam Bartholomew

Computer Science Summer Practicum 2016

## Troubleshooting

#### 500 Server Error when updating Activity Images

This is usually a folder permissions problem. The Json site runs as cct.service@gordon.edu. To solve this issue, edit the permissions to allow the cct.service@gordon.edu user to edit the `browseable` folder. The folder is located in the Api site folder (either 360Api or 360ApiTrain, depending on which is having the problem).

Note that the permissions are reset everytime a new `browseable` folder is created. This should not usually happen because the deployment scripts don't touch the `browseable` folder. However, in the case that you delete the old `browseable` folder and put a new one in, make sure to also edit the permissions.


#### 500 Server errors appear all of a sudden, even when nothing has changed in the code base.

At this point, I think we eleminiated most code base problems. Potential code base errors will be throwing custom exceptions that will tell you more about what is wrong. If it is a plain 500 error though, the problem might be a database one.
Check:
- That the ACT_INFO and ACT_CLUB_DEF tables are in sync.
- That the stored procedures return exactly what the models expect.
- That the views are up. Sometimes CTS unexpectedly does maintainance. Try running simple select statements against the Views.



