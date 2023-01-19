## The Database

The `CCT` and `MyGordon` databases exist in:

- `admintrainsql.gordon.edu` - The development/testing database server
- `adminprodsql.gordon.edu` - The live production database server.

### CCT Tables

All the tables were created from scratch by our team.

Misc Information:

- Apart from a few exceptions, the tables don't make use of foreign key constraints. This is because the relevant primary keys are in the tables referenced by Views. Unfortunately, one cannot make foreign keys that reference Views.

#### ACT_INFO

A record in this table stores:

- ACT_CDE - The short code for the activity.
- ACT_DESC - The name of the activity.
- ACT_BLURB - A short description of what the activity is about. This will be filled out by a leader.
- ACT_URL - URL to the website for the club/organization (if they have one).
- ACT_IMAGE_PATH - Path to where the activity logo is stored in the browseable folder.
- ACT_TYPE - Short code for the type of the activity
- ACT_TYPE_DESC - Full name of the type of the activity

You might notice that this table is an extension of the ACT_CLUB_DEF view. It contains extra information that the view does not have, but that we need. This is clearly a case of Information Duplication; information is available in two places and can easily fall out of sync. To remedy this, the stored procedure [UPDATE_ACT_INFO](#update_act_info) was made.

#### ADMIN

A record in this table stores:

- ID_NUM - The gordon id number of the administrator
- USER_NAME - The administrator's username
- EMAIL - The administrator's email.
- SUPER_ADMIN - Whether or not the admin has super admin privilege

To make someone an admin, simply insert a record into this table through MSSQL Studio.

#### CUSTOM_PROFILE

A record in this table stores:

- username - The gordon username of the current user (firstname.lastname)
- facebook - The URL of the user's facebook without its domain name
- twitter - The URL of the user's twitter without its domain name
- instagram - The URL of the user's instagram without its domain name
- linkedin - The URL of the user's linkedin without its domain name
- handshake - The URL of the user's handshake without its domain name (just user id)

Users don't exist in the table unless they add/edit their social media links on 360 site. Once a user adds any links, the user will be added to the table. This logic is done so that there won't be unused users in the table which can possibly slow down the website.

#### MYSCHEDULE

A record in this table stores:

- EVENT_ID - The event id number of this schedule (always has to be above 1000, to differentiate between a course schedule)
- GORDON_ID - The gordon id number of the user having this event
- LOCATION - The location of the event
- DESCRIPTION - The description of the event
- MON_CDE - Whether or not the event is in monday ('M')
- TUE_CDE - Whether or not the event is in tuesday ('T')
- WED_CDE - Whether or not the event is in wednesday ('W')
- THU_CDE - Whether or not the event is in thursday ('R')
- FRI_CDE - Whether or not the event is in friday ('F')
- SAT_CDE - Whether or not the event is in saturday ('S')
- SUN_CDE - Whether or not the event is in sunday ('N')
- IS_ALLDAY - Whether or not the event is happening for all day '0' for no and '1' for all day.
- BEGIN_TIME - The start time of the event in Timespan format
- END_TIME - The end time of the event in Timespan format

Myschedules doesn't exist in the table unless a user add/edit myschedule on 360 site. Once a user adds any customized event, the event will be added to the table. The structure is adopted from the course schedule format stored in other database. There are two primary keys - EVENT_ID and GORDON_ID. They have to match together to get any event schedule

#### Schedule_Control

A record in this table stores:

- IsSchedulePrivate - Whether or not the schedule is private (only applied to students and their course schedule. FacStaff and mySchedule won't be affected)
- ModifiedTimeStamp - The last time when the user modified the event or description
- Description - The schedule description for additional links
- gordon_id - The gordon id number of the current user

Schedule Controls also don't exist in the table unless a user add/edit their settings on 360 site.

#### JNZB_ACTIVITIES

A record in this table stores all the same fields as an Activity table in Jenzabar would.
The goal of this table was to contain membership information that was to be moved to Jenzabar. To do this, one would use the stored procedure [UPDATE_JNZB_ACTIVITIES](#update_jnzb_activities).

#### MEMBERSHIP

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

#### REQUEST

A record in this table stores:

- SESS_CDE - The short code for the session they are requesting to join.
- ACT_CDE - The short code of the activity they are requesting to join.
- ID_NUM - The gordon id number of the potential member.
- PART_CDE - The short code for the participation level they want to join as.
- DATE_SENT - The date the request was made.
- COMMENT_TXT - Comments to accompany the request.
- STATUS - Status of the request. Should be either Pending, Approved or Denied.\

#### ACT_CLUB_DEF

A record in this table stores

- ACT_CDE - The activity short code.
- ACT_DESC - The name of the activity.

This table is an exact duplicate of the JENZ_ACT_CLUB_DEF view. It is periodically updated by making sure what is in it corresponds to what is in JENZ_ACT_CLUB_DEF. When a new activity is found in JENZ_ACT_CLUB_DEF, it is inserted into ACT_CLUB_DEF and the stored procedure UPDATE_ACT_INFO is run.

### MyGordon Tables

These are just the MyGordon tables used by 360. They were originally not made for this site.

##### StudentNews

A record in this table stores

- SNID - unique integer identifier for a news entry
- ADUN - username (first.last) of the person who posted the entry
- categoryID - foreign key that links this entry to its category
- Subject - subject, written by the poster, of the news entry
- Body - the actual text of the news entry, written by the poster
- Image - the path to the image (if there is one) associated with the news item
- Accepted - whether this entry has been approved to be shown publicly
- Sent - whether the item has been sent
- thisPastMailing - whether it belongs to this past mailing
- Entered - when, in datetime format, the post was submitted by the poster
- fname - not used (NULL)
- lname - not used (NULL)
- ManualExpirationDate - given by the poster, the last day on which this entry should be displayed publicly

##### StudentNewsCategory

A record in this table stores

- categoryID - a unique integer identifier
- categoryName - the name of a category, ex. "Found Items"
- SortOrder - an integer representing the category's placement in the preferred display order

### CCT Views

We got access to these views through CTS. They are a direct live feed from the tables they represent. As mentioned earlier, we cannot use primary keys in the views to make foreign keys in other tables.

#### ACCOUNT

Account information for all the members of gordon college.

#### Alumni

The Alumni information. Includes their information the same way as students.

#### Buildings

Descriptions of the different codes for buildings around campus.

#### CHAPEL_EVENT

Information on chapel attendance for every student

#### JENZ_ACT_CLUB_DEF

The Activity information. Includes short codes and what they represent.

#### Countries

Descriptions of different codes for countries.

#### CM_SESSION_MSTR

The Session information. Includes short codes, the session they represent, and the physical dates spanned by the session.

#### FacStaff

A subset of `ACCOUNT` that has only faculty and staff member records.

#### Majors

Descriptions of the different codes for majors.

#### ALL_BASIC_INFO

Pulls firstname, lastname, category (student, staff, faculty), and AD_Username (if it exists!) and then makes a concatenated string to be searched through

#### PART_DEF

Definitions of the different participation levels for someone in an activity.

#### Police

A list of IDs that are identified as gordon police.

#### Student

A subset of `ACCOUNT` that has only student records.

#### 360_SLIDER

Content (images, captions, and links) for the slider on the dashboard page.

### CCT Stored Procedures

Stored procedures have been written to make some database accesses and administrative tasks easier.
Here are the most important ones.

#### EVENTS_BY_STUDENT_ID

Returns all events which a student has attended based upon their AD_Username

#### ALL_BASIC_INFO

Pulls firstname, lastname, category (student, staff, faculty), and AD_Username (if it exists!) and then makes a concatenated string to be searched through

#### ADVISOR_SEPARATE

This stored procedures is simple. It returns separate advisor(s) ID (Max is 3, Advisor1, Advisor2, and Advisor 3) for a particular student based upon their ID.

#### UPDATE_ACT_CLUB_DEF

This keeps the ACT_CLUB_DEF table in sync with the JENZ_ACT_CLUB_DEF view. It should be run periodically.

#### UPDATE_ACT_INFO

Because ACT_INFO is basically a duplicate of ACT_CLUB_DEF, this stored procedure tries to keep them synced. Ideally it should be run automatically anytime ACT_CLUB_DEF changes.

In non-sql terms, this procedure makes sure all the activities defined in ACT_CLUB_DEF are also present in ACT_INFO. If something has been added/removed to ACT_CLUB_DEF but not in ACT_INFO, it adds/removes the corresponding record to ACT_INFO.

#### UPDATE_JNZB_ACTIVITIES

This stored procedures is pretty simple. It moves all the relevant information from the MEMBERSHIP table and puts it in the JNZB_ACTIVITIES table. To prevent duplication, it will only add records that are present in the MEMBERSHIP table, but missing the JNZB_ACTIVITIES table.

### CCT Triggers

#### ACT_CLUB_DEF_INSERT_TRIGGER

Every time a record is inserted into the ACT_CLUB_DEF table, this trigger runs the UPDATE_ACT_INFO stored procedure. Although not clear in the name, this trigger also runs whenever a row is deleted from ACT_CLUB_DEF.

### Manual and Debugging Access

It's sometimes useful to look at the database directly, to see the schema or check data. Here is how.

- [Use remote desktop to get to the Windows server VM](RemoteDesktopToVM.md)

- Open SQL Server Management Studio (SSMS)

- Connect to "ADMINTRAINSQL" database server (or "ADMINPRODSQL")
- Expand "Databases" then "CCT" (primary database), "MyGordon", "StudentTimesheets", or "TmsEPrd" then "Views" or "Tables"

- To see schemas, expand "dbo." entries and their "columns"
- To see data, right-click a view and select "Select top 1000 rows"
