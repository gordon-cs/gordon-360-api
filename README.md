# Gordon 360

### (IN PROGRESS...)

Dive in.
## Table of Contents
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
    - [Running the tests](#running-the-tests)
        - [Mac](#mac)
        - [PC](#pc)


`API Url: ` Coming soon...


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
- `gordon_360_tests_god.py` -- Coming Soon...
- `test_config.py` -- Configuration options, includes the following variables:
    - `activity_code` -- The activity that will be used for testing. Tests under `gordon_360_tests_leader.py` assume the account used for testing is a leader of this activity. Tests under `gordon_360_tests_member.py` assume the account used for testing is a member of this activity.
    - `random_id_number` -- A random id number that is used when we want to verify if we can do things on behalf of someone else. E.g. A supervisor can create memberships for anyone. A regular member can only create a membership for him/herself.
    - `leadership_positions` -- A list of participation levels considered to be leadership positsions.
    - `hostURL` -- Base url of the api

### Setting up

If the project was cloned from Github, an additional file is needed.
- Create a file called `test_credentials.py`.
- In the file, define the following variables.
	- `username` -- String with the username of a test account that is a member of `activity_code` in `test_config.py`.
	- `password` -- String with the password of a test account that is a member of `activity_code` in `test_config.py`.
	- `id_number` -- Integer with the id number of the `username`.
	- `username_leader` -- String with the username of a test account that is a leader of `activity_code` in `test_config.py`.
	- `password_leader` -- String with the password of a test account that is a leader of `activity_code` in `test_config.py`.
	- `id_number_leader` -- Integer with the id number of the `username_leader`.

### Running the tests

Make sure your machine has python3

#### Mac
From inside the ApiEndpoints directory, run the following commands:
```Shell
sudo pip install -r requirements.txt
python gordon_360_tests_member.py
```
This will run the tests in `gordon_360_tests_member.py`. You can run the other tests by specifying their names instead. You can run all tests by using `python gordon_360_tests.py`.

#### PC

Coming soon....

Team members: Eze Anyanwu, James Kempf, Adam Bartholomew

Computer Science Summer Practicum 2016

