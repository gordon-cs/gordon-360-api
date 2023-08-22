using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gordon360.Models.ViewModels;

public record MembershipHistoryViewModel(string ActivityCode,
                                         string ActivityDescription,
                                         string ActivityImagePath,
                                         IEnumerable<MembershipHistorySessionViewModel> Sessions,
                                         DateTime LatestDate)
{
    public static MembershipHistoryViewModel FromMembershipGroup(IGrouping<string, MembershipView> membershipGroup)
    {
            var first = membershipGroup.First();

            var sessions = membershipGroup.Aggregate(
                Enumerable.Empty<MembershipHistorySessionViewModel>(),
                (sessions, membership) => sessions.Append(new MembershipHistorySessionViewModel(
                    membership.MembershipID,
                    membership.SessionCode,
                    membership.Participation)));

            return new MembershipHistoryViewModel(
                ActivityCode: membershipGroup.Key,
                ActivityDescription: first.ActivityDescription,
                ActivityImagePath: first.ActivityImagePath,
                Sessions: sessions,
                LatestDate: membershipGroup.Max(m => m.EndDate ?? m.StartDate)
            );
    }
}

public record MembershipHistorySessionViewModel(int MembershipID, string SessionCode, string Participation);
