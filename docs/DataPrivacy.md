# Data Privacy Documentation
This document explains the classes of data hosted on 360 and who they should be available to.

## Approach
360 follows a zero-trust policy of locking down data unless specifically notated that a user should have access to that data due to their campus role. If at any point a developer is unsure whether a user "class" should have access to a piece of data or not, they should default back to granting no access until they are able to confirm otherwise with the proper authority.

## Privacy Settings
There are three possible privacy settings: _Private_, _FacStaff_, and _Public_.  These may be applied to profile data and these may have different meaning depending on the class of the viewer.

### Students
- The field _KeepPrivate_ contains a string and may be NULL.  As of 2024-05-28 the only non-NULL value in the 360 Train DB is 'S'.
- The field _isMoblePhonePrivate_ contains an integer - either 0 or 1.  This is treated as a boolean to indicate if the mobile phone number should be shown to other students.  **NOTE:** Faculty/Staff can always see the phone number but the text color indicates the student's desired privacy setting.

### Faculty/Staff Table
- The field _KeepPrivate_ contains an integer - either 0 or 1.  This is treated as a boolean to indicate if the home address (town & state) and martial status/spouse name should be shown on their profile.

### Alumni
- The field _ShareAddress_ contains a string and may be NULL or contain "Y" or "N".  **NOTE:** As of 2024-05-28 this field being NULL is ambiguous - sometimes NULL is intepreted to mean "not Y" and other times it means "not N".
- The field _ShareName_ contains a string and may be NULL or contain "Y" or "N".

## Data Entities

### Searching
#### Students
- Can search for students
#### Faculty/Staff
- Can search for students
#### Alumni
#### Public

### Profiles

#### Students
- Can view other students who are not marked as "FERPA Private" (uncommon).
- Can view facstaff information unless marked as _Private_ or _FacStaff_
- Can view alumni
##### Faculty/Staff
- Can view basic student information & more specific info based on job role requirements (RAs, SFS, Police, etc.).
- Can view extended information of advisees [I'm not sure that this is actually in practice for any data].
- Can view other facstaff information unless marked as private
##### Alumni
- Can view facstaff information unless marked as private
- Can view other alumni basic information
- Cannot view active student information
##### Public (unauthenticated, resigned facstaff, retired faculty, incoming students)
- Cannot view any profile information for any account

### Events

##### Students/Faculty/Staff
- Can view all events

##### Alumni/Public
- Can view events marked as public
