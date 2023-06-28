# Data Privacy Documentation
This document explains the classes of data hosted on 360 and who they should be available to.

## Approach
360 follows a zero-trust policy of locking down data unless specifically notated that a user should have access to that data due to their campus role. If at any point a developer is unsure whether a user "class" should have access to a piece of data or not, they should default back to granting no access until they are able to confirm otherwise with the proper authority.

## Data Entities

### Profiles

##### Students
- Can view other students who are not marked as "FERPA Private" (uncommon).
- Can view facstaff information unless marked as private
- Cannot view alumni
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
