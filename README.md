
# Gordon 360

#### The API consumed by [gordon-360-ui](https://github.com/gordon-cs/gordon-360-ui)
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
- [Caching](#caching)
- [API Endpoints](#api-endpoints)
    - [Accounts](#accounts)
    - [Activities](#activities)
    - [Admins](#admins)
    - [Authentication](#authentication)
    - [Content Management](#content-management)
    - [Emails](#emails)
    - [Events](#events)
    - [Memberships](#memberships)
    - [Membership Requests](#membership-requests)
    - [Participation Definitions](#participation-definitions)
    - [Profiles](#profiles)
    - [Sessions](#sessions)
- [API Testing](#api-testing)
    - [Introduction](#introduction)
    - [Running the Tests](#running-the-tests)
    - [Manual Testing](#manual-testing)
- [Troubleshooting](#troubleshooting)
- [Documentation](#documentation)

## Machines and Sites
As of Summer 2018 the virtual machines CS-RDSH-01 and CS-RDSH-02 are used for developing Gordon 360.  Instructions for connecting via Remote Desktop can be found in [RemoteDesktopToVM.md](RemoteDesktopToVM.md).

To work on this project, it is easiest to use the following machines provided by CTS:
- 360train.gordon.edu - Windows machine.
    - Can be accessed through Remote Desktop Connection.
    - Has the C# code.
    - Has Visual Studio, MSSQL Server.
    - Has deployment scripts and folders.
    - Has site folders.
- CS-360-API-TEST.gordon.edu - Ubuntu machine
    - Is accessed through ssh.
    - Is setup for running the tests (has an already filled `test_credentials.py` file.)
    - Has the User-facing code (HTML, JS and CSS)


The folders for these IIS sites can be found on the 360train machine under `F:\sites`.
- 360.gordon.edu -- Production Front-end. User-facing code (css, js, html)
- 360Train.gordon.edu -- Development Front-end. User-facing code (css, js, html)
- 360Api.gordon.edu -- Production JSON server site. C# using the ASP.NET Framework.
- 360ApiTrain.gordon.edu -- Development JSON server site. C# using the ASP.NET Framework.

### Deploying to the Api Site
- Log in to cts-360.gordon.edu as the cct.service user and start Visual Studio as an administrator (right click).
- Open an existing project/solution - `C:\users\cct.service\code\gordon-360-api\Gordon360` file. It is a Microsoft Visual Studio Solution file.
- Make a change. Do your thing.
- Menu Bar -> Build - Publish Gordon360.
- Choose the right publish profile.
    - DEV -- Development ( Connects to the admintrainsql database server, and used for 360train.gordon.edu).
    - Prod -- Production ( Connects to the adminprodsql database server, and used for the real site 360.gordon.edu).
- Clicking publish pushes your changes to the API for either 360ApiTrain.gordon.edu or 360Api.gordon.edu, depending on which publish profile you used.

### Deploying to the Front-end Site
**Note: these instructions are out-of-date, since Project Bernard is deprecated with the transition to gordon-360-ui!** Please refer to the documentation for [gordon-360-ui](https://github.com/gordon-cs/gordon-360-ui) on GitHub. The new gordon-360-ui front-end uses the React framework rather than Ember.

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

## Caching

Since the type of solution we are using does not run like many systems, we have to cache a request that occurs every few minutes after startup. As such, we have implemented code in the startup.cs file that:
 1 ) Performs static methods and saves the output to a static object (located in Helpers and Data, respectively)
 2 ) Create an entry in the cache that then runs these static methods every few minutes

This process makes use of static names, methods, and data, since ASP.NET does not use global variables.

Data which is stored upon startup includes:

- All events in 25Live ending after the start of the current academic year
- All basic information on every account with an AD Username
- All student, faculty, staff and alumni profile info

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

###### CUSTOM_PROFILE

A record in this table stores:

- username - The gordon username of the current user (firstname.lastname)
- facebook - The URL of the user's facebook without its domain name
- twitter - The URL of the user's twitter without its domain name
- instagram - The URL of the user's instagram without its domain name
- linkedin - The URL of the user's linkedin without its domain name

Users don't exist in the table unless they add/edit their social media links on 360 site. Once a user adds any links, the user will be added to the table. This logic is done so that there won't be unused users in the table which can possibly slow down the website.

###### JNZB_ACTIVITIES

A record in this table stores all the same fields as an Activity table in Jenzabar would.
The goal of this table was to contain membership information that was to be moved to Jenzabar. To do this, one would use the stored procedure [UPDATE_JNZB_ACTIVITIES](#update_jnzb_activities).

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

### Stored Procedures

Stored procedures have been written to make some database accesses and administrative tasks easier.
Here are the most important ones.

###### EVENTS_BY_STUDENT_ID
Returns all events which a student has attended based upon their AD_Username

###### ALL_BASIC_INFO
Pulls firstname, lastname, category (student, staff, faculty), and AD_Username (if it exists!) and then makes a concatenated string to be searched through

###### UPDATE_ACT_CLUB_DEF

This keeps the ACT_CLUB_DEF table in sync with the JENZ_ACT_CLUB_DEF view. It should be run periodically.

###### UPDATE_ACT_INFO

Because ACT_INFO is basically a duplicate of ACT_CLUB_DEF, this stored procedure tries to keep them synced. Ideally it should be run automatically anytime ACT_CLUB_DEF changes.

In non-sql terms, this procedure makes sure all the activities defined in ACT_CLUB_DEF are also present in ACT_INFO. If something has been added/removed to ACT_CLUB_DEF but not in ACT_INFO, it adds/removes the corresponding record to ACT_INFO.


###### UPDATE_JNZB_ACTIVITIES

This stored procedures is pretty simple. It moves all the relevant information from the MEMBERSHIP table and puts it in the JNZB_ACTIVITIES table. To prevent duplication, it will only add records that are present in the MEMBERSHIP table, but missing the JNZB_ACTIVITIES table.


### Triggers

###### ACT_CLUB_DEF_INSERT_TRIGGER

Every time a record is inserted into the ACT_CLUB_DEF table, this trigger runs the UPDATE_ACT_INFO stored procedure. Although not clear in the name, this trigger also runs whenever a row is deleted from ACT_CLUB_DEF.

## The Code

### Introduction

The server was written using ASP.NET and is generally structured as such. As a MVC (Model View Controller) system, the heart of the code is in ApiControllers (which is organized like the API it implements, which is documented later in this file) and in the Models folder. The View is provided by a separate repository, gordon-360-ui.

Here is a breakdown of the project folder:

- gordon-360-api/
    - Design_Documents/ - currently empty. I do not actually remember why we had this.
    - Gordon360/ - The main project. Most of the work will be done here.
        - ApiControllers/ - Folder containing the Controllers for the API endpoints.
        - AuthorizationFilters/ - Contains code that enforces rules about who can access what.
        - AuthorizationServer/ (The folder should really be called AuthenticationServer) -  Contains code that does user authentication.
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

## API Endpoints

### Accounts
What is it? Resource that represents a gordon account.

##### GET

`api/accounts/:email` Get the account with email `email`.

`api/accounts/:username` Get the account with `username`.

`api/accounts/search/:searchString` Returns the basicinfoviewmodel with a Concatenated attribute matching some or all of the searchstring

`api/accounts/advanced-people-search/{includeAlumniSearchParam}/{firstNameSearchParam}/{lastNameSearchParam}/{majorSearchParam}/{minorSearchParam}/{classTypeSearchParam}/{hometownSearchParam}/{stateSearchParam}/{countrySearchParam}/{departmentSearchParam}/{buildingSearchParam}` Get all the accounts matching the specified parameters. Access to accounts is based on your account type (e.g. Students can't get Alumni).

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

##### PUT

`api/activities/:id/session/{sess_cde}/close` Close out an activity for a given session (this is like confirming the final roster of an activity for a given session.

`api/activities/:id/session/{sess_cde}/open` Reopen an activity for a given session.

`api/activities/:id` Edit activity information for the club with activity code `id`.

`api/activities/:id/private/:p` Update a given activity to private or not private with boolean value `p`. The `id` parameter is the activity id.


### Admins
What is it? Resource that represents admins.

Who has access? Only super admins, except to get a specific admin where all admins have access.

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

`api/advanced-search/states` Get all states that are found in the Student, Alumni, and FacStaff tables.

`api/advanced-search/countries`  Get all countries that are found in the Student, Alumni, and FacStaff tables.
 
`api/advanced-search/departments` Get all the departments from the FacStaff table.

`api/advanced-search/buildings` Get all the buildings from the FacStaff table.



### Authentication

##### POST

`/token:` The authentication process uses a simplified version of what is called Open Authentication (OAuth).

In OAuth, there are two servers including the one running your app. The server running your app doesn't authenticate people directly, it relies on the second server to tell it if a given person is allowed access. This second server is called the Authentication Server.

In our project, the Authentication Server and the App Server are actually the same. They are only separated code-wise. You could say that the *App* is the ApiControllers folder and the *Authentication Server* is the AuthorizationServer folder (Recall that it is currently named incorrectly, sorry. We haven't changed the name because it would require changing multiple import statements, and we don't have time to debug after the change).

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

### Profiles
What is it? Resource that represents users' profiles.

Differences from GoSite:
- Only displaying city and country as home address. (When the viewer is a student. Police, super admin, faculty and staff should still see all the information for home address)
- Displaying minors.
- On campus was changed to display more general information rather than completely getting rid of it like GoSite does now. (Shows on/off campus)

##### GET

`api/profiles` Get profile info of the current logged in user.

`api/profiles/:username` Get profile info of a user with username `username` as a parameter.

`api/profiles/role/:username` Get college role of a user with username `username` as a parameter --- College roles: super admin, faculty and staff, student and police.

`api/profiles/Image/` Get profile image of the current logged in user. Image is stored in a base 64 string.

`api/profiles/Image/:username` Get the profile image(s) of a user with username `username` as a parameter. Image is stored in a base 64 string. Police, super admin, faculty and staff can view both default and preferred profile image of students. Only police and super admin can view both images of everyone including faculty and staff.

##### POST

`api/profiles/image` Upload a preferred image for the current logged in user.

`api/profiles/image/reset` Delete preferred image and set profile image to default for the current logged in user.

`api/profiles/:type` Update a social media link of a type(facebook, twitter, linkedin,instagram) of current logged in user.

##### PUT

`api/profiles/mobile_privacy/:value` Update mobile phone number privacy with value(Y or N) for the current logged in user.

`api/profiles/image_privacy/:value` Update profile image privacy with value(Y or N) for the current logged in user.


### Emails
What is it? Resource that represents emails.


##### GET

`api/emails/activity/:id` Get the emails for members of the activity with activity code `id` during the current session.

`api/emails/activity/:id/session/:sessionid` Get the emails for the members of the activity with activity code `id` during the session with session code `sessionid`.

`api/emails/activity/:id/leaders` Get the emails for the leaders of the activity with activity code `id` during the current session.

`api/emails/activity/:id/leaders/session/:sessionid` Get the emails for the leaders of the activity with activity code `id` during the session with session code `sessionid`.

`api/emails/activity/:id/advisors` Get the emails for the advisors of the activity with activity code `id` during the current session.

`api/emails/activity/:id/advisors/session/:sessionid` Get the emails for the advisors of the activity with activity code `id` during the session with session code `sessionid`.


### Events
What is it? Resources to get information on Events from the 25Live system
- Only confirmed events are pulled
- Only events ending after the start of the current academic year are requested from 25Live
- Data from 25Live is retrieved every four minutes using a cached request

##### GET

`api/events/chapel/:user_name` Get all events attended by a student (pulls from local database)

`api/events/chapel/:user_name/:term` Get all events attended by a student in a specific term

`api/events/25Live/type/:Type_ID` Get event(s) specified by a type ID (or multiple). A full list can be found here: https://webservices.collegenet.com/r25ws/wrd/gordon/run/evtype.xml?parent_id=9&otransform=browse.xsl
Multiple types or events are separated by a '$'

`api/events/25Live/:Event_ID` Get event(s) specified by one or multiple Event_ID. Event IDs can be found in the url or resources in a 25Live request in a browser.
Multiple types or events are separated by a '$'

`api/events/25Live/All` Returns all events in 25Live under predefined categories.

`api/events/25Live/CLAW` Returns all events in 25Live with Category_ID = 85 (CL&W Credit approved)


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

`api/memberships/activity/:id/members` Get the number of members of an activity with activity code `id`.

`api/memberships/activity/:id/followers/:sess_cde` Get the number of followers of an activity with activity code `id` in session `:sess_cde`.

`api/memberships/activity/:id/members/:sess_cde` Get the number of members of an activity with activity code `id` in session `:sess_cde`.

`api/memberships/student/:id` Get the memberships of the student with student id `id`.

`api/memberships/student/username:username` Get the public version of memberships of the student with student username `username`.

##### POST

`api/memberships` Create a new membership.


##### PUT

`api/memberships/:id` Edit the membership with membership id `id`.

`api/memberships/:id/group-admin` Toggle whether or not a given member is in a group admin role for a given activity. The `id` parameter is the membership id.

`api/memberships/:id/private/:p` Update a given membership to private or not private with boolean value `p`. The `id` parameter is the membership id.

##### DELETE

`api/memberships/:id` Delete the membership with membership id `id`.

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


### Profiles
What is it? Resource that represents users' profiles.

Differences from GoSite:
- Only displaying city and country as home address. (When the viewer is a student. Police, super admin, faculty and staff should still see all the information for home address)
- Displaying minors.
- On campus was changed to display more general information rather than completely getting rid of it like GoSite does now. (Shows on/off campus)

##### GET

`api/profiles` Get profile info of the current logged in user.

`api/profiles/:username` Get profile info of a user with username `username` as a parameter.

`api/profiles/role/:username` Get college role of a user with username `username` as a parameter --- College roles: super admin, faculty and staff, student and police.

`api/profiles/Image/` Get profile image of the current logged in user. Image is stored in a base 64 string.

`api/profiles/Image/:username` Get the profile image(s) of a user with username `username` as a parameter. Image is stored in a base 64 string. Police, super admin, faculty and staff can view both default and preferred profile image of students. Only police and super admin can view both images of everyone including faculty and staff.

##### POST

`api/profiles/image` Upload a preferred image for the current logged in user.

`api/profiles/image/reset` Delete preferred image and set profile image to default for the current logged in user.

`api/profiles/:type` Update a social media link of a type(facebook, twitter, linkedin,instagram) of current logged in user.

##### PUT

`api/profiles/mobile_privacy/:value` Update mobile phone number privacy with value(Y or N) for the current logged in user.

`api/profiles/image_privacy/:value` Update profile image privacy with value(Y or N) for the current logged in user.


### Sessions
What is it? Resource that represents the current session. e.g. Fall 2014-2015.

Who has access? Everyone.

##### GET

`api/sessions` Get all the sessions.

`api/sessions/:id` Get the session with session code `id`.

`api/sessions/current` Get the current session.

`api/sessions/daysLeft` Get the days left in the semester and the total days in the semester


## API Testing

### Introduction

A test suite is available at `Tests/ApiEndpoints` to exercise the different endpoints. The most important files here are:
- `gordon_360_tests_leader.py` -- Tests the api endpoints while authorized as an activity leader.
- `gordon_360_tests_member.py` -- Tests the api endpoints while authorized as a regular member.
- `test_config.py` -- Configuration options, includes the following variables:
    - `activity_code` -- The activity that will be used for testing. Tests under `gordon_360_tests_leader.py` assume the account used for testing is a leader of this activity. Tests under `gordon_360_tests_member.py` assume the account used for testing is a member of this activity.
    - `random_id_number` -- A random id number that is used when we want to verify if we can do things on behalf of someone else. E.g. An advisor can create memberships for anyone. A regular member can only create a membership for him/herself.
    - `leadership_positions` -- A list of participation levels considered to be leadership positions.
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
`git clone https://github.com/gordon-cs/gordon-360-api.git`

Navigate to the API Tests folder:
`cd gordon-360-api/Tests/ApiEndpoints/`

Install the python request module as well as other requirements with the command: `pip install -r requirements.txt`

Create the `test_credentials.py` file (or copy it with `cp /c/users/public/documents/test_credentials.py .` -- notice the trailing dot).  It should define the six variables mentioned above (`username`, `password`, `id_number`, etc.)

Make sure the credentials you enter match the descriptions provided above.

Verify that the variables defined in `test_config.py` are correct.

Run the tests:
`python3 gordon_360_tests.py` -- This runs all the tests. For both members and leaders.
`python3 gordon_360_tests_member.py` -- This runs the tests for members.
`python3 gordon_360_tests_leader.py` -- This runs the tests for leaders.

### Manual Testing

#### Running the Server Locally

* As you are probably using one of the Linux machines in the Computer Science lounge, you will need to be on the virtual machine to run the server locally. Follow the directions [here](RemoteDesktopToVM.md) to set up and connect to the virtual machine.

* If this is your first time on the virtual machine, you will need to clone the 360 code. You can use something like Git Bash or VS Code to do this.

* Before you open the gordon-360-api folder, you will have to add the `secrets.config` file to it. The file is located on the CS-RDSH-02 virtual machine in `C:\Users\Public\Public Documents\` (or `/c/users/public/documents\` when in git-bash). Copy the file `secrets.config` to the same folder in your project that contains the `web.config` file; currently, this is in `gordon-360-api\Gordon360`. This file is a sort of keyring for the server to authorize itself at various points.

* Now, to open the api, look for the desktop app Visual Studio 2017, which has a purple Visual Studio icon. You will have to log in to a Microsoft account, which can just be the account Gordon made for you. Once you log in, go to `File > Open > Project/Solution`. Then, select and Open the file `gordon-360-api/Gordon360.sln`. 

* There is a little configuration you must yet do before running the server. In the solution explorer on the right, right click the name of the project (Gordon360) and select properties.  From the tabs on the left, choose the Web tab and change the Project Url to an unused port. For example, if you chose port 5555, change Project Url to `"http://localhost:5555"`. Then click Create Virtual Directory. Press OK on the dialog box, and you all configured!

* Now, you can press the Start button in Visual Studio to run the server (it is a green play button in the top middle of the tool bar). It will open the web browser and, after a period that may last half an hour or more, display an Error 403.14 - Forbidden. This is expected. You can now begin manually testing the API.

* If you want to test the UI, keep the server running and follow the directions found [here](https://github.com/gordon-cs/gordon-360-ui/blob/develop/README.md#connect-local-backend-to-react) under "Connect Local Backend to React". 

#### Manually Testing the API

To manually test the API, use an API development/testing app like [Postman](https://www.getpostman.com/).
* Here you can create HTTP requests to hit the API endpoints that you want to test, and see what data response you get back.
* _Before you can call any normal API endpoints_, you must first call the authentication endpoint with a PUT request, which will give you a token.  After starting Postman, use the following steps:
	* Near the top of the workspace window, change the request type from "GET" to "PUT" using the drop-down menu
	* Enter the local back-end URL with `/token` appended.  It should look something like `http://localhost:5555/token`, but use the port number you selected when carrying out the steps under [Running the server locally](#running-the-server-locally) rather than 5555
	* Just below the URL you entered, click on "Body"
	* Check the radio-button for `x-www-form-urlencoded`
	* Enter the following three pairs in the Key/Value fields, replacing _username_ and _password_ with valid data:

    | Key          | Value         |
    |--------------|---------------|
    | `grant_type` | `password` |
    | `username`   | _username_ |
    | `password`   | _password_ |

	* Click the blue "Send" button - after a brief pause you should see the returned token appear.
* You can use this token to make an API request.  For example:
	* Use the clipboard to make of copy of the _access-token_ value (do not include the double quotes, just copy the long string of characters between the quotes)
	* Click on the "+" tab near the top of the window to open a new request frame
	* Leave "GET" as the request type and enter in an appropriate API URL (e.g. `http://localhost:5555/api/memberships/activity/AJG`
	* Just below the URL, click on "Headers" and enter the following key/value pairs replacing _access-token_ with the token string you copied:

    | Key             | Value                               |
    |-----------------|-------------------------------------|
    | `Content-Type`  | `application/x-www-form-urlencoded` |
	| `Authorization` | `Bearer ` _access-token_            |

	* Click the blue "Send" button


## Troubleshooting

#### 500 Server Error when updating Activity Images

This is usually a folder permissions problem. The Json site runs as the user cct.service@gordon.edu. To solve this issue, edit the permissions to allow the cct.service@gordon.edu user to edit the `browseable` folder. The folder is located in the Api site folder (either 360Api or 360ApiTrain, depending on which is having the problem).

Note that the permissions are reset every time a new `browseable` folder is created. This should not usually happen because the deployment scripts don't touch the `browseable` folder. However, in the case that you delete the old `browseable` folder and put a new one in, make sure to also edit the permissions.


#### 500 Server errors appear all of a sudden, even when nothing has changed in the code base.

At this point, I think we eliminated most c#-related problems. Potential c#-related errors will be throwing custom exceptions that will tell you more about what is wrong. If you do get plain 500 error though, the problem might be a database one.
Check:
- That the ACT_INFO and ACT_CLUB_DEF tables are in sync.
- That the stored procedures return exactly what the models expect.
- That the views are up. Sometimes CTS unexpectedly does maintenance. Try running simple select statements against the Views.

#### 404 Not Found when trying to access the `/token` endpoint:

This error will only pop up when you are testing the server directly by running it with visual studio. When you run the gordon360 server from visual studio it automatically "hosts" it on `localhost:3000`. By default, the server doesn't accept non-HTTPS (anything not on port 443) connections. There are two solutions:
- Change Visual studio settings to run on `localhost:443` by default. I tried this a bit, but didn't get very far. I don't think it is that hard though, I just didn't have the motivation to continue.
- Allow non-HTTPS connections by commenting out some code. This is what I did. DON'T FORGET TO REMOVE THE COMMENT SYMBOLS AFTER YOU FINISH THOUGH.
    - The code that restricts non-HTTPS connections is located under the `Startup.cs` file. Look for the "#if DEBUG" and "#endif" code-blocks. Comment both out.


### Documentation
The documentation folder currently contains the ColdFusion files from go.gordon that contain the logic behind the people search.
* Index.cfm is the page used to select search criteria.
* Searchresults.cfm is the list of people you get back based on that criteria.  It selects from a student view, facstaff view, and alumni view all separately and then sorts all the results together.
* Showperson.cfm is the detail page of the person you select from the searchresults.cfm page.

Team members: Bradley Boutcher, Chris Qiao, Jenny Kim, Joe Ross, Matt Felgate, Sam Nguyen

Computer Science Summer Practicum 2017
