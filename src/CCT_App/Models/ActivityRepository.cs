using System;
using System.Collections.Generic;
using System.Collections.Concurrent;


namespace cct_api.models
{
    public class ActivityRepository : IActivityRepository
    {
        static ConcurrentDictionary<string, Activity> _activities = 
            new ConcurrentDictionary<string, Activity>();

        public ActivityRepository()
        {
            Add(new Activity { 
                activity_id = "activity_id_1",
                activity_description = "A test activity",
                activity_name = "Frisbee Golf",
                activity_advisor = "Mr. John" 
                });

            Add(new Activity { 
                activity_id = "activity_id_2",
                activity_description = "A second test activity",
                activity_name = "Go Club",
                activity_advisor = "Mr. Smith" 
                });
            
            Add(new Activity { 
                activity_id = "activity_id_3",
                activity_description = "A third test activity",
                activity_name = "Fun club",
                activity_advisor = "Mrs. Hi" 
                });

            Add(new Activity { 
                activity_id = "activity_id_4",
                activity_description = "A fourth test activity",
                activity_name = "Club",
                activity_advisor = "Miss. Yo" 
                });
        }

        public IEnumerable<Activity> GetAll()
        {
            return _activities.Values;
        }

        public void Add(Activity activity)
        {
            
            _activities[activity.activity_id] = activity;
        }
        public Activity Find(string id)
        {
            Activity activity;
            _activities.TryGetValue(id, out activity);
            return activity;
        }

        public Activity Remove(string id)
        {
            Activity activity;
            _activities.TryGetValue(id, out activity);
            _activities.TryRemove(id, out activity);
            return activity;
        }

        public void Update(Activity activity)
        {
            _activities[activity.activity_id] = activity;
        }

    }

}