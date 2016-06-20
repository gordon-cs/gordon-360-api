using System.Collections.Generic;

namespace cct_api.models
{
    public interface IActivityRepository
    {
        void Add(Activity activity);
        IEnumerable<Activity> GetAll();
        Activity Find(string id);
        Activity Remove(string id);
        void Update(Activity item);
    }
}