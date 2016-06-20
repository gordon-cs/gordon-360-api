using System.Collections.Generic;
using System.Collections.Concurrent;


namespace cct_api.models
{
    public class ActivityRepository : IActivityRepository
    {
        static ConcurrentDictionary<string, Activity> _activities = 
            new ConcurrentDictionary<string, Activity>();

        public ActivityRepository()
        {}

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