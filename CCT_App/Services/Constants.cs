using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCT_App.Services
{
    public static class Constants
    {
        public static readonly string[] LeaderParticipationCodes =
        {
            "AC","CAPT","CODIR",
            "CORD","DIREC","PRES",
            "RA1","RA2","RA3",
            "VICEC","VICEP"
        };

        public const string getMembershipForActivityQuery = @"SELECT MEMBERSHIP_ID as MembershipID,
		                                                    mem.ACT_CDE as ActivityCode,
		                                                    act.ACT_DESC as ActivityDescription,
		                                                    mem.SESSION_CDE as SessionCode,
		                                                    sess.SESS_DESC as SessionDescription,
		                                                    mem.ID_NUM as IDNumber,
		                                                    acct.firstname as FirstName,
		                                                    acct.lastname as LastName,
		                                                    mem.PART_LVL as Participation,
		                                                    part.PART_DESC as ParticipationDescription,
		                                                    mem.BEGIN_DTE as StartDate,
		                                                    mem.END_DTE as EndDate,
		                                                    mem.DESCRIPTION as Description
                                                        FROM Membership as mem
                                                        INNER JOIN ACT_CLUB_DEF as act
	                                                        ON mem.ACT_CDE = act.ACT_CDE
                                                        INNER JOIN CM_SESSION_MSTR as sess
	                                                        ON mem.SESSION_CDE = sess.SESS_CDE
                                                        INNER JOIN ACCOUNT as acct
	                                                        ON mem.ID_NUM = acct.gordon_id
                                                        INNER JOIN PART_DEF as part
	                                                        ON mem.PART_LVL = part.PART_CDE
                                                        WHERE mem.ACT_CDE = @p0";
    }
}