## Structure of the API 

### Resources:
- Activities
- Memberships
- ActivityAdvisors

#### Activities:

`/activities` -- List of all activity objects e.g. [{"activity_id":0,"activity_name":"Go Club"},{"activity_id":1,"activity_name":"Dialogue Club}].

`/activities/:id` -- Activity object with the provided ID e.g. {"activity_id":0,"activity_name":"Go Club","activity_advisor":"Joe Smith", "description":"Come play GO"}.

`/activities/:id/students` -- List of all students in selected activity e.g. [{"student_id":0, "student_name":"Guy Person"},{"student_id":1, "student_name":"Girl Person"}].

#### Memberships:

`/membership` -- List of all memberships e.g. [{"membership_id":0, "student_id":0, "activity_id":0},{"membership_id":1, "student_id":1}].

`/membership/:id` -- Membership object with the provided ID e.g. {"membership_id":0, "student_id":0, "activity_id":0, "activity_name":"Go Club", "description":"Come play GO", "start_date":"May 2000", "end_date":"June 2000", affiliation:"Member"}


#### Students:

`/students` -- List of all students e.g. [{"student_id":0, "student_name":"Eze Anyanwu"}, { ... }]

`/students/:id` -- Student object e.g. {"student_id":0, "student_name":"Eze Anyanwu"}.

`/students/:id/activities` -- List of activities student is involved in e.g. {"student_id":0, "activities":[{"activity_id":0,"activity_name":"Go Club"},{ ... }]}


#### ActivityAdvisors:

`/activity-advisors` -- List of all activity advisors e.g. [{"supervisor_id":0, "faculty_id":235, "supervisor_name":"Staff Person"}, { ... }]

`/supervisor/:id` -- Supervisor object with the provided ID e.g. {"supervisor_id":0, "faculty_id":235, "supervisor_name":"Staff Person"}

`/supervisor/:id/activities` [supervisor_id:0, activities:[{"activity_id":0,"activity_name":"Go Club"},{ ... }]
