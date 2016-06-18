using System;
using System.Collections.Generic;
using System.Collections.Concurrent;


namespace cct_api.models
{
    public class MembershipRepository : IMembershipRepository
    {
        static ConcurrentDictionary<string, Membership> _memberships = 
            new ConcurrentDictionary<string, Membership>();

        public MembershipRepository()
        {}

        public IEnumerable<Membership> GetAll()
        {
            return _memberships.Values;
        }

        public void Add(Membership membership)
        {
            
            _memberships[membership.membership_id] = membership;
        }
        public Membership Find(string id)
        {
            Membership membership;
            _memberships.TryGetValue(id, out membership);
            return membership;
        }

        public Membership Remove(string id)
        {
            Membership membership;
            _memberships.TryGetValue(id, out membership);
            _memberships.TryRemove(id, out membership);
            return membership;
        }

        public void Update(Membership membership)
        {
            _memberships[membership.membership_id] = membership;
        }

    }

}