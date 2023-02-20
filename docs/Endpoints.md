# API Endpoints

This documentation explains the different endpoints of the API.

## Legacy

NOTE: these docs are no longer maintained and are likely to be missing information or else wholly incorrect. They are only being preserved until we have a better human-readable source of documentation.

This info ought to be redundant with the route docs in the [XML documentation](Gordon360/Documentation/Gordon360.xml) automatically generated from the documentation comments in our code. However, since the documentation comments are not as human-readable, and since considerable effort was put into these endpoint docs, we are choosing to preserve this document in the short-term.

## Notation

In the API endpoint descriptions below, some parameter values are indicated by a leading ":" (though convention is to surround the parameter with {curly-brackets}).  The ":" is not present in the URL, just the value.  Also, a trailing "/" is required after the last parameter value if the parameter includes a special character like a period (e.g. `360.studenttest` necessitates trailing slash).

_Note:_ The shell script `get-route-list.sh` is run with `bash get-route-list.sh` from a linux shell or git-bash. It provides a list of the API routes that appear in the ApiController files.

## Accounts

What is it? Resource that represents a gordon account.

#### GET

`api/accounts/email/{email}` Get the account with email `email`. Currently restricted to FacStaff, Police, and Group Admins.

`api/accounts/username/{username}` Get the account with `username`. Currently restricted to FacStaff, Police, and Group Admins.

`api/accounts/search/{searchString}` Returns the basicinfoviewmodel with a Concatenated attribute matching some or all of the searchstring

`api/accounts/search/{searchString}/{secondaryString}` The same as above, used when the search string contains a space

`api/accounts/advanced-people-search/{includeStudentSearchParam}/{includeFacStaffSearchParam}/{includeAlumniSearchParam}/{firstNameSearchParam}/{lastNameSearchParam}/{majorSearchParam}/{minorSearchParam}/{hallSearchParam}/{classTypeSearchParam}/{hometownSearchParam}/{stateSearchParam}/{countrySearchParam}/{departmentSearchParam}/{buildingSearchParam}` Get all the accounts matching the specified parameters. Access to accounts is based on your account type (e.g. Students can't get Alumni).

## Activities

What is it? Resource that represents some activity - such as a club, ministry, leadership program, etc.

#### GET

`api/activities` Get all the activities.

`api/activities/{id}` Get the activity with activity code `id`.

`api/activities/session/{id}` Get the activities offered during the session with session code `id`.

`api/activities/session/{id}/types` Get the different activity types among the activities offered during the session with session code `id`.

`api/activities/{sessionCode}/{id}/status` Get the status of an activity (either open or closed), which indicates whether or not new members can be added to the activity for this session.

`api/activities/open` Get all the open activities for the current session.

`api/activities/{id}/open` Get only the open activities for which a given user (identified by their user `id`) is the group admin.

`api/activities/closed` Get all the closed activities for the current session.

`api/activities/{id}/closed` Get only the closed activities for which a given user (identified by their user `id`) is the group admin.

#### PUT

`api/activities/{id}/session/{sess_cde}/close` Close out an activity for a given session (this is like confirming the final roster of an activity for a given session).

`api/activities/{id}/session/{sess_cde}/open` Reopen an activity for a given session.

`api/activities/{id}` Edit activity information for the club with activity code `id`.

`api/activities/{id}/privacy/{p}` Update a given activity to private or not private with boolean value `p`. The `id` parameter is the activity id.

#### POST

`api/activities/{id}/image` Set an image for the activity with activity code `id`.

`api/activites/{id}/image/reset` Reset the image to default for the activity with activity code `id`.

## Admins

What is it? Resource that represents admins.

Who has access? Only super admins, except to get a specific admin where all admins have access.

NOTE: facultytest is a super admins in PRODAPIDATA, stafftest is a super admins in TRAINAPIDATA

#### GET

`api/admins` Get all the admins.

`api/admins/{id}` Get a specific admin with the Gordon ID specified.

#### POST

`api/admins` Create a new admin.

#### DELETE

`api/admins/{id}` Delete the admin with the admin id `id`.

## Advanced Search

#### GET

`api/advanced-search/majors` Get all majors that are found in the Student table.

`api/advanced-search/minors` Get all minors that are found in the Student table.

`api/advanced-search/halls` Get all halls that are found in the Student table.

`api/advanced-search/states` Get all states that are found in the Student, Alumni, and FacStaff tables.

`api/advanced-search/countries` Get all countries that are found in the Student, Alumni, and FacStaff tables.

`api/advanced-search/departments` Get all the departments from the FacStaff table.

`api/advanced-search/buildings` Get all the buildings from the FacStaff table.

## Authentication

#### POST

`/token` The authentication process uses a simplified version of what is called Open Authentication (OAuth).

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

## Content Management

What is it? Resource for fetching content that has been stored in the database by Gordon's website [content manager](http://wwwtrain.gordon.edu/).

#### GET

`api/cms/slider` Get the content for the dashboard slide.

## Emails

What is it? Resource that represents emails.

#### GET

`api/emails/activity/{id}` Get the emails for members of the activity with activity code `id` during the current session.

`api/emails/activity/{id}/session/{sessionid}` Get the emails for the members of the activity with activity code `id` during the session with session code `sessionid`.

`api/emails/activity/{id}/leaders` Get the emails for the leaders of the activity with activity code `id` during the current session.

`api/emails/activity/{id}/leaders/session/{sessionid}` Get the emails for the leaders of the activity with activity code `id` during the session with session code `sessionid`.

`api/emails/activity/{id}/group-admin/session/{sessionid}` Get the emails for the group admins of the activity with activity code `id` during the session with session code `sessionid`.

`api/emails/activity/{id}/advisors` Get the emails for the advisors of the activity with activity code `id` during the current session.

`api/emails/activity/{id}/advisors/session/{sessionid}` Get the emails for the advisors of the activity with activity code `id` during the session with session code `sessionid`.

#### PUT

`api/emails` Sends an email.

`api/emails/activity/{id}/session/{sessionid}` Sends an email to the participants of the activity with activity code `id` during the session with session code `sessionid`.

`api/emails/activity/{id}/leaders/session/{sessionid}` Sends an email to the leaders of the activity with activity code `id` during the session with session code `sessionid`.

## Events

What is it? Resources to get information on Events from the 25Live system

- Only confirmed events are pulled
- Only events with event_type_id 14 `Calendar Announcement` or 57 `Event`.
- Only events ending after the start of the current academic year are requested from 25Live
- Data from 25Live is retrieved every four minutes using a cached request

#### GET

`api/events/chapel/{term}` Get all events attended by the logged-in user in a specific term

`api/events/25Live/All` Returns all events in 25Live under predefined categories.

`api/events/25Live/CLAW` Returns all events in 25Live with Category_ID = 85 (CL&W Credit approved).

`api/events/25Live/Public` Returns all events in 25Live marked to promote on public calendars (Requirement_ID = 3).

## Housing

What is it? Resource that represents residence hall information that would concern the residents or housing director.

#### GET

`api/housing/admin` Gets an Http OK if the current user is in the admin whitelist table and gets Not Found otherwise

`api/housing/halls/apartments` Gets the list of apartment-style halls that are available for student housing and gets Not Found if it cannot be found

`api/housing/apartment/` Gets the application ID matching the current user if that user is on an application saved for the current semester, otherwise returns a Not Found code

`api/housing/apartment/{username}/` Gets the application ID matching the given username if that user is on an application saved for the current semester, otherwise returns a Not Found code

`api/housing/apartment/applications/{applicationID}/` Gets the apartment application info matching the given application ID if it is found in the database, otherwise returns a Not Found code

`api/housing/admin/apartment/applications` Gets the apartment application info for all applications submitted during the current semester, otherwise returns a Not Found code

#### PUT

`apartment/applications/{applicationID}/submit` Changes the date an application was submitted (changes it from null the first time they submit)

#### DELETE

`apartment/applications/{applicationID}` Deletes an application (and consequently all rows that reference it)

## Jobs

What is it? Resource to interact with a student's timesheets for on-campus jobs.

#### GET

`api/jobs` Get the active jobs of the current logged in user.
`api/jobs/getSavedShift` Get the saved shifts of the current logged in user.
`api/jobs/supervisorName/{supervisorID}` Get the name of a supervisor based on their ID number `supervisorID` as a parameter.
`api/jobs/clockOut` Get the time-clock status of the current logged in user, true if user is clocked in, and false if clocked out.

#### POST

`api/jobs/saveShift` Create a new shift for the current logged in user.
`api/jobs/submitShifts` Submit all shifts for the current logged in user.
`api/jobs/clockIn` Update the time-clock status of the current logged in user, true if user is clocked in, and false if clocked out.

#### PUT

`api/jobs/editShift` Edit a shift for the current logged in user.
`api/jobs/deleteClockIn` Delete the last clocked in status of a user.

#### DELETE

`api/jobs/deleteShift/{rowID}` Delete a shift with row id `rowID`.

## Memberships

What is it? Resource that represents the affiliation between a student and a club.

#### GET

`api/memberships` Get all the memberships.

`api/memberships/{id}` Get the membership with membership id `id`.

`api/memberships/activity/{id}` Get the memberships associated with the activity with activity code `id`.

`api/memberships/activity/{id}/leaders` Get the memberships of the leaders for the activity with activity code `id`.

`api/memberships/activity/{id}/advisors` Get the memberships of the advisors for the activity with activity code `id`.

`api/memberships/activity/{id}/group-admin` Get the memberships of the group admin (displayed as "Group Contacts") for the activity with activity code `id`.

`api/memberships/activity/{id}/followers` Get the number of followers of an activity with activity code `id`.

`api/memberships/activity/{id}/members` Get the number of members (excluding followers) of an activity with activity code `id`.

`api/memberships/activity/{id}/followers/{sess_cde}` Get the number of followers of an activity with activity code `id` in session `sess_cde`.

`api/memberships/activity/{id}/members/{sess_cde}` Get the number of members (excluding followers) of an activity with activity code `id` in session `sess_cde`.

`api/memberships/student/{id}` Get the memberships of the student with student id `id`.

`api/memberships/student/username/{username}` Get the public version of memberships of the student with student username `username`.

`api/memberships/isgroupadmin/{id}` Get whether or not a student with id `id` is a Group Admin for some activity. Service method is used for security purposes but Controller is currently just for testing convenience.

#### POST

`api/memberships` Create a new membership.

#### PUT

`api/memberships/{id}` Edit the membership with membership id `id`.

`api/memberships/{id}/group-admin` Toggle whether or not a given member is in a group admin role for a given activity. The `id` parameter is the membership id.

`api/memberships/{id}/privacy/{p}` Update a given membership to private or not private with boolean value `p`. The `id` parameter is the membership id.

#### DELETE

`api/memberships/{id}` Delete the membership with membership id `id`.

## Membership Requests

What is it? Resource that represents a person's application/request to join an activity group.

#### GET

`api/requests` Get all the membership applications.

`api/requests/{id}` Get the membership application with request id `id`.

`api/requests/student/{id}` Get all the membership applications for the student with student id `id`.

`api/requests/activity/{id}` Get all the membership applications for the club with activity code `id`.

#### PUT

`api/requests/{id}` Edits an existing memberships application.

#### POST

`api/requests` Create a new membership application.

`api/requests/{id}/deny` Deny the membership application with request id `id`.

`api/requests/{id}/approve` Approve the membership application with request id `id`.

#### DELETE

`api/requests/{id}` Delete the membership application with id `id`.

## Sessions

What is it? Resource that represents the current session. e.g. Fall 2014-2015.

Who has access? Everyone.

#### GET

`api/sessions` Get all the sessions.

`api/sessions/{id}` Get the session with session code `id`.

`api/sessions/current` Get the current session.

`api/sessions/firstDay` Get the Gets the first day in the current session.

`api/sessions/lastDay` Get the Gets the last day in the current session.

`api/sessions/daysLeft` Get the days left in the semester and the total days in the current session.

## Participation Definitions

What is it? Resource that represents the different levels with which a person can affiliate themselves with a club.

Who has access? Everyone.

#### GET

`api/participations` Get all the possible participation levels.

`api/participations/{id}` Get the participation level with code `id`.

`api/participations/leaders` Get the participation levels that are considered leaders.

`api/participations/transcript-worthy` Get the participation levels that should appear on the cct as leadership. Unfinished: TO DO.

## Profiles

What is it? Resource that represents users' profiles.

Differences from GoSite:

-   Only displaying city and country as home address. (When the viewer is a student or alumni. Police, super admin, faculty, and staff should still see all the information for home address)
-   Displaying minors.
-   On campus was changed to display more general information rather than completely getting rid of it like GoSite does now. (Shows on/off campus)

#### GET

`api/profiles` Get profile info of the current logged in user.

`api/profiles/{username}` Get public profile info of a user with username `username` as a parameter.

`api/profiles/Advisors/{username}` Get advisor(s) info of a user with username `username` as a parameter.

`api/profiles/clifton/{username}` Get the Clifton Strengths of a user with username `username` as a parameter.

`api/profiles/mailbox-combination` Get the mailbox combination of the current logged in user.

`api/profiles/Image` Get profile image of the current logged in user. Image is stored in a base 64 string.

`api/profiles/Image/{username}` Get the profile image(s) of a user with username `username` as a parameter. Image is stored in a base 64 string. Police, super admin, faculty and staff can view both default and preferred profile image of students. Only police and super admin can view both images of everyone including faculty and staff.

#### POST

`api/profiles/image` Upload a preferred image for the current logged in user.

`api/profiles/IDimage` Submit an ID image for the current logged in user.

`api/profiles/image/reset` Delete preferred image and set profile image to default for the current logged in user.

`api/profiles/{type}` Update a social media link of a type (Facebook, Twitter, LinkedIn, Instagram, Handshake) of current logged in user.

#### PUT

`api/profiles/mobile_phone_number/{value}` Update mobile phone number for the current logged in user.

`api/profiles/mobile_privacy/{value}` Update mobile phone number privacy with value (Y or N) for the current logged in user.

`api/profiles/image_privacy/{value}` Update profile image privacy with value (Y or N) for the current logged in user.

## Dining

What is it? Request meal plan info and current balances.

Who has access? Everyone.

#### GET

`api/dining` Get all possible meal plan info.

- If user has one or more meal plans, then current balances for each plan are included in a JSON response.
- If user does not have a meal plan but has a faculty-staff dining balance, then the balance is returned as a string.
- If there is no plan or balance then the string "0" (equivalent to no balance) is returned.

## Student Employment

What is it? A resource that represents the campus employments of the currently logged in user.

#### GET

`api/studentemployment` Get the record of campus employments for the currently logged in user.

## Schedule

What is it? Resource that represents a course schedule of user.

#### GET

`api/schedule` Get all schedule objects of the currently logged in user.

`api/schedule/{username}` Get all schedule objects of a user with username `username` as a parameter.

## My Schedule

What is it? Resource that represents a customized schedule of user.

#### GET

`api/myschedule` Get all custom events of the currently logged in user.

`api/myschedule/{username}` Get all custom events of a user with username `username` as a parameter.

`api/myschedule/event/{eventId}` Get a specific custom event of the currently logged in user with `eventId` as a parameter

#### PUT

`api/myschedule/` Update a custom event of the currently logged in user.

#### POST

`api/myschedule/` Create a custom event of the currently logged in user.

#### DELETE

`api/myschedule/{eventID}` Delete a custom event of the currently logged in user.

## Schedule Control

What is it? Resource that represents information related to schedule.

#### GET

`api/schedulecontrol` Get the schedulecontrol object of the currently logged in user. Specifically, get the privacy, time last updated, description, and Gordon ID of the currently logged in user's schedule.

`api/schedulecontrol/{username}` Get the schedulecontrol object of a user with username `username` as a parameter. Specifically, Get the privacy, time last updated, description, and Gordon ID of the user's schedule.

#### PUT

`api/schedulecontrol/privacy/{value}` Update a schedule privacy of the currently logged in user.

`api/schedulecontrol/description/{value}` Update a schedule description of the currently logged in user.

## Victory Promise

What is it? Resource that represents the user's scores on the four pillars of the victory promise.

#### GET

`api/vpscore` Get the victory promise scores of the currently logged in user.

## News

What is it? Resource that represents accepted student news entries and news categories.

#### GET

`api/news/{newsID}` Gets a student news item from the database.

`api/news/{newsID}/image` Gets the base64 data for an image associated with a student news entry.

`api/news/category` Gets the full list of category names used to categorize student news as well as category ids and sort order.

`api/news/not-expired` Gets every student news entry that has been accepted and has not yet been in the database 2 weeks or, if the poster set a specific date of expiration, has an expiration date later than the current day.

`api/news/new` Gets every student news entry that has been accepted, has not expired (as described above), and is new since 10am on the day before.

`api/news/personal-unapproved` Gets all of the unexpired and unapproved news submissions by the authenticated user
_(uses stored procedure)_

#### POST

`api/news` Submits a news item into the database (initally unapproved)
_(uses stored procedure)_

#### DELETE

`api/news/{id}` Deletes a news item from the database by its id (SNID = student news id). In order to delete, the following conditions must be met:

- news item is unexpired
- user is author of news item or SUPER_ADMIN (perhaps should be changed in future)

_(uses repository)_

#### PUT

`api/news/{id}` Edits a news item in the database by its id. In order to edit, the following conditions must be met:

- news item is unexpired
- user is author of news item or SUPER_ADMIN (perhaps should be changed in future)
- news item has not yet been approved

_(uses repository)_

## Wellness Check

Back endpoint responsible for fetching and sending information to the database regarding the answers to the wellness check.

#### GET

`api/wellness` Gets the latest answer a student has sent, as well as a boolean that specifies whether the answer is still valid based on when the answer was submitted.

`api/wellness/Question` Gets the wellness check question to be displayed on the front end from the Data base.

#### POST

`api/wellness` Sends an answer boolean to the database that specifies whether a student is symptomatic or not: true = symptomatic, false = not symptomatic.

## Academic Check-In

What is it? Framework responsible for fetching and sending information to the database relating to a student's Academic Check-In. 

#### GET 

`api/checkIn/holds` Gets a list of student's current academic holds and returns them to the front end.

`api/checkIn/status` Gets a student's current check-in status to determine if they need to complete check-in.

#### Post

`api/checkIn/emergencycontact` Posts a student's emergency contact data to the database for storage.

#### Put

`api/checkIn/cellphone` Updates a student's personal cellphone data in the database.

`api/checkIn/demographic` Updates a student's demographic data (race/ethnicity) in the database.

`api/checkIn/status` Updates a student's check-in status to mark them as completed.
