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

`api/memberships/:id` Get the membership with id `id`.

`api/memberships/activity/:id` Get the memberships associated with the activity with id `id`.

`api/memberships/activity/:id/leaders` Get the memberships of the leaders for the activity with id `id`.

`api/memberships/student/:id` Get the memberships of the student with student id `id`.

##### POST

`api/memberships` Create a new membership.

##### PUT 

`api/memberships/:id` Edit the membership with id `id`.

##### DELETE

`api/memberships/:id` Delete the membership with id `id`.


### Clubs
What is it? Resource that represents a club.
Who has access? It's complicated.

##### GET

`api/activities` Get all the clubs.

`api/activities/:id` Get the club with id `id`.

`api/activities/session/:id` Get the clubs offered during the session with id `id`.

##### PUT

`api/activities/:id` Edit club information for the club with id `id`.

### Membership Requests
What is it? Resource that represents a person's application/request to join a club.
Who has access? It's complicated.

##### GET

`api/requests` Get all the membership applications.

`api/requests/:id` Get the membership application with id `id`.

`api/requests/student/:id` Get all the membership applications for the student with id `id`.

`api/requests/activity/:id` Get all the membership applications for the club with id `id`.


##### POST

`api/requests` Create a new membership application.

`api/requests/:id/deny` Deny the membership application with id `id`.

`api/requests/:id/approve` Approve the membership application with id `id`.

##### DELETE

`api/requests/:id` Delete the membership application with id `id`.

### Supervisors
What is it? Resource that represents the supervisor of an activity.
Who has access? It's complicated.

##### GET

`api/supervisors`

`api/supervisors/:id`

`api/supervisors/activity/:id`

##### POST

`api/supervisors`

#### PUT

`api/supervisors/:id`

##### DELETE

`api/supervisors/:id`


### Students
What is it? Resource that represents a student.
Who has access? Probably not you.

##### GET

`api/students`

`api/students/:id`

`api/student/:email`


### Accounts
What is it? Resource that represents a gordon account.
Who has access? Probably not you.

##### GET

`api/accounts/:email`

### Sessions
What is it? Resource that represents the current session. e.g. Fall 2014-2015.
Who has access? Everyone.

##### GET

`api/sessions`

`api/sessions/:id`

`api/sessions/current`

### Participation Definitions
What is it? Resource that represents the different levels with which a person can affiliate themselves with a club.
Who has access? Everyone.

##### GET

`api/participations`

`api/partiipations/:id`


### Emails
What is it? Resource that represents emails. 
Who has access? It's complicated.


##### GET 

`api/emails/activity:id`

`api/emails/activity:id/leaders`




Team members: Eze Anyanwu, James Kempf, Adam Bartholomew

Computer Science Summer Practicum 2016

