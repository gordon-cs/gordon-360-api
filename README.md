# Gordon 360

Dive in.

`API Url: ` Coming soon...


## API Endpoints: (IN PROGRESS ... )

### Authentication

#### POST

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

#### GET

`/memberships`

`/memberships/:id`

`/memberships/activity/:id`

`/memberships/activity/:id/leaders`

`/memberships/student/:id`

#### POST

`/memberships`

#### PUT 

`/memberships`

#### DELETE

`/memberships/:id`


### Clubs

#### GET

`/activities`

`/activities/session/:id`

### Membership Requests

#### GET

`/requests`

`/requests/student/:id`

`/requests/activity/:id`


#### POST

`/requests`

#### DELETE

`/requests/:id`

### Sessions

#### GET

`/sessions`

`/sessoins/current`

### Participation Definitions

#### GET

`/participations`



Team members: Eze Anyanwu, James Kempf, Adam Bartholomew

Computer Science Summer Practicum 2016

