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
-- Description:	Get all Requests
-- =============================================
CREATE PROCEDURE ALL_REQUESTS 
	-- Add the parameters for the stored procedure here

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
SELECT req.REQUEST_ID as RequestID,
		req.ACT_CDE as ActivityCode,
		activity.ACT_DESC as ActivityDescription,
		req.ID_NUM as IDNumber,
		acct.firstname as FirstName,
		acct.lastname as LastName,
		req.PART_CDE as Participation,
		part.PART_DESC as ParticipationDescription,
        sess.SESS_CDE as SessionCode,
		sess.SESS_DESC as SessionDescription,
		req.COMMENT_TXT as CommentText,
		req.DATE_SENT as DateSent,
		req.STATUS as RequestApproved
FROM REQUEST as req
INNER JOIN CM_SESSION_MSTR as sess
	ON req.SESS_CDE = sess.SESS_CDE
INNER JOIN ACT_CLUB_DEF as activity
	ON req.ACT_CDE = activity.ACT_CDE
INNER JOIN ACCOUNT as acct
	ON req.ID_NUM = acct.gordon_id
INNER JOIN PART_DEF as part
	ON req.PART_CDE = part.PART_CDE

END
GO
