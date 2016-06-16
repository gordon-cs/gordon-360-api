using System.Collections.Generic;

namespace cct_api.models
{
    public interface IMembershipRepository
    {
        void Add(Membership membership);
        IEnumerable<Membership> GetAll();
        Membership Find(string id);
        Membership Remove(string id);
        void Update(Membership item);
    }
}