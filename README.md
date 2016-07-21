# Gordon 360

Dive in.

`API Url: ` Coming soon...


## API Endpoints: (IN PROGRESS ... )

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

`api/partiipations/:id` Get the participation level with code `id`.


### Emails
What is it? Resource that represents emails. 

Who has access? It's complicated.


##### GET 

`api/emails/activity:id` Get the emails for members of the activity with activity code `id`.

`api/emails/activity:id/leaders` Get the emails for the leaders of the activity with activity code `id`.




Team members: Eze Anyanwu, James Kempf, Adam Bartholomew

Computer Science Summer Practicum 2016

