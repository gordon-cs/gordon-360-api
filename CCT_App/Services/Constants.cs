using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCT_App.Services
{
    /// <summary>
    /// This class is mostly being used for constructing sql queries. Since the main tables this application relies on are actually views,
    /// no relationships are declared between the tables in EF. This means that I don't have access to navigational properties. Without navigational
    /// properties, I can't construct helpful view models. When an end user recieves a Membership response for example, they would expect to see
    /// fields like ActivityName and First and Last names. If I'm only given them back database models, they will only see fieldss like MembershipID
    /// and ActivityCode, which are not so helpful. 
    /// So to remedy this, I am using SQL joins here to get the relevant fields I would need to populate my view models.
    /// </summary>
    public static class Constants
    {
        

        public static readonly string[] LeaderParticipationCodes =
        {
            "AC","CAPT","CODIR",
            "CORD","DIREC","PRES",
            "RA1","RA2","RA3",
            "VICEC","VICEP"
        };
        public const string getMembershipsForStudentQuery = @"SELECT MEMBERSHIP_ID as MembershipID,
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
                                                    WHERE mem.ID_NUM = @p0";

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

        public const string getLeadersForActivityQuery = @"SELECT MEMBERSHIP_ID as MembershipID,
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
                                                    WHERE mem.ACT_CDE = @p0 AND mem.PART_LVL in ('AC','CAPT','CODIR','CORD','DIREC','PRES','RA1','RA2','RA3','VICEC','VICEP')";

        public const string getSupervisorsForActivityQuery = @"SELECT sup.SUP_ID as SupervisorID,
		                                                    acct.firstname as FirstName,
		                                                    acct.lastname as LastName,
		                                                    sup.ID_NUM as IDNumber,
		                                                    activity.ACT_CDE as ActivityCode,
		                                                    activity.ACT_DESC as ActivityDescription,
		                                                    sess.SESS_CDE as SessionCode,
		                                                    sess.SESS_DESC as SessionDescription	
                                                    FROM SUPERVISOR as sup
                                                    INNER JOIN ACCOUNT as acct
	                                                    ON sup.ID_NUM = acct.gordon_id
                                                    INNER JOIN CM_SESSION_MSTR as sess
	                                                    ON sup.SESSION_CDE = sess.SESS_CDE
                                                    INNER JOIN ACT_CLUB_DEF as activity
	                                                    ON sup.ACT_CDE = activity.ACT_CDE
                                                    WHERE sup.ACT_CDE = @p0";

        public const string getMembershipByIDQuery = @"SELECT MEMBERSHIP_ID as MembershipID,
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
                                        WHERE mem.MEMBERSHIP_ID = @p0";

        public const string getAllMembershipsQuery = @"SELECT MEMBERSHIP_ID as MembershipID,
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
	                                        ON mem.PART_LVL = part.PART_CDE";

        public const string getAllMembershipRequestsQuery = @"SELECT	req.REQUEST_ID as RequestID,
		                                                    req.ACT_CDE as ActivityCode,
		                                                    activity.ACT_DESC as ActivityDescription,
		                                                    req.ID_NUM as IDNumber,
		                                                    acct.firstname as FirstName,
		                                                    acct.lastname as LastName,
		                                                    req.PART_LVL as Participation,
		                                                    part.PART_DESC as ParticipationDescription,
                                                            sess.SESS_CDE as SessionCode,
		                                                    sess.SESS_DESC as SessionDescription,
		                                                    req.COMMENT_TXT as CommentText,
		                                                    req.DATE_SENT as DateSent,
		                                                    req.APPROVED as RequestApproved
                                                    FROM Request as req
                                                    INNER JOIN CM_SESSION_MSTR as sess
	                                                    ON req.SESS_CDE = sess.SESS_CDE
                                                    INNER JOIN ACT_CLUB_DEF as activity
	                                                    ON req.ACT_CDE = activity.ACT_CDE
                                                    INNER JOIN ACCOUNT as acct
	                                                    ON req.ID_NUM = acct.gordon_id
                                                    INNER JOIN PART_DEF as part
	                                                    ON req.PART_LVL = part.PART_CDE";


        public const string getMembershipRequestByIdQuery = @"SELECT req.REQUEST_ID as RequestID,
		                                                    req.ACT_CDE as ActivityCode,
		                                                    activity.ACT_DESC as ActivityDescription,
		                                                    req.ID_NUM as IDNumber,
		                                                    acct.firstname as FirstName,
		                                                    acct.lastname as LastName,
		                                                    req.PART_LVL as Participation,
		                                                    part.PART_DESC as ParticipationDescription,
                                                            sess.SESS_CDE as SessionCode,
		                                                    sess.SESS_DESC as SessionDescription,
		                                                    req.COMMENT_TXT as CommentText,
		                                                    req.DATE_SENT as DateSent,
		                                                    req.APPROVED as RequestApproved
                                                    FROM Request as req
                                                    INNER JOIN CM_SESSION_MSTR as sess
	                                                    ON req.SESS_CDE = sess.SESS_CDE
                                                    INNER JOIN ACT_CLUB_DEF as activity
	                                                    ON req.ACT_CDE = activity.ACT_CDE
                                                    INNER JOIN ACCOUNT as acct
	                                                    ON req.ID_NUM = acct.gordon_id
                                                    INNER JOIN PART_DEF as part
	                                                    ON req.PART_LVL = part.PART_CDE
                                                    WHERE req.REQUEST_ID = @q0";

        public const string getSupervisorByIdQuery = @"SELECT sup.SUP_ID as SupervisorID,
		                                            acct.firstname as FirstName,
		                                            acct.lastname as LastName,
		                                            sup.ID_NUM as IDNumber,
		                                            activity.ACT_CDE as ActivityCode,
		                                            activity.ACT_DESC as ActivityDescription,
		                                            sess.SESS_CDE as SessionCode,
		                                            sess.SESS_DESC as SessionDescription	
                                            FROM SUPERVISOR as sup
                                            INNER JOIN ACCOUNT as acct
	                                            ON sup.ID_NUM = acct.gordon_id
                                            INNER JOIN CM_SESSION_MSTR as sess
	                                            ON sup.SESSION_CDE = sess.SESS_CDE
                                            INNER JOIN ACT_CLUB_DEF as activity
	                                            ON sup.ACT_CDE = activity.ACT_CDE
                                            WHERE sup.SUP_ID = @q0";

        public const string getAllSupervisorsQuery = @"SELECT sup.SUP_ID as SupervisorID,
		                                            acct.firstname as FirstName,
		                                            acct.lastname as LastName,
		                                            sup.ID_NUM as IDNumber,
		                                            activity.ACT_CDE as ActivityCode,
		                                            activity.ACT_DESC as ActivityDescription,
		                                            sess.SESS_CDE as SessionCode,
		                                            sess.SESS_DESC as SessionDescription	
                                            FROM SUPERVISOR as sup
                                            INNER JOIN ACCOUNT as acct
	                                            ON sup.ID_NUM = acct.gordon_id
                                            INNER JOIN CM_SESSION_MSTR as sess
	                                            ON sup.SESSION_CDE = sess.SESS_CDE
                                            INNER JOIN ACT_CLUB_DEF as activity
	                                            ON sup.ACT_CDE = activity.ACT_CDE";
    }

}