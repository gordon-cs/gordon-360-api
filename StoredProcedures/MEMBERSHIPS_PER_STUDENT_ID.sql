-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Eze Anyanwu
-- Create date: 07/19/2016
-- Description:	Select the memberships associated with the student.
-- =============================================
CREATE PROCEDURE MEMBERSHIPS_PER_STUDENT_ID 
	-- Add the parameters for the stored procedure here
	@STUDENT_ID int = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
SELECT MEMBERSHIP_ID as MembershipID,
		mem.ACT_CDE as ActivityCode,
		act.ACT_DESC as ActivityDescription,
		act_info.ACT_IMAGE as ActivityImage,
		act_info.MTG_DAY as ActivityMeetingTime,
		act_info.MTG_TIME as ActivityMeetingDay,
		mem.SESS_CDE as SessionCode,
		sess.SESS_DESC as SessionDescription,
		mem.ID_NUM as IDNumber,
		acct.firstname as FirstName,
		acct.lastname as LastName,
		acct.email as Email,
		mem.PART_CDE as Participation,
		part.PART_DESC as ParticipationDescription,
		mem.BEGIN_DTE as StartDate,
		mem.END_DTE as EndDate,
		mem.COMMENT_TXT as Description
FROM MEMBERSHIP as mem
INNER JOIN ACT_CLUB_DEF as act
	ON mem.ACT_CDE = act.ACT_CDE
INNER JOIN ACT_INFO as act_info
	ON mem.ACT_CDE = act_info.ACT_CDE
INNER JOIN CM_SESSION_MSTR as sess
	ON mem.SESS_CDE = sess.SESS_CDE
INNER JOIN ACCOUNT as acct
	ON mem.ID_NUM = acct.gordon_id
INNER JOIN PART_DEF as part
	ON mem.PART_CDE = part.PART_CDE
WHERE mem.ID_NUM = @STUDENT_ID

END
GO
