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
-- Description:	Get all supervisors
-- =============================================
CREATE PROCEDURE ALL_SUPERVISORS 
	-- Add the parameters for the stored procedure here

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
SELECT sup.SUP_ID as SupervisorID,
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
	ON sup.SESS_CDE = sess.SESS_CDE
INNER JOIN ACT_CLUB_DEF as activity
	ON sup.ACT_CDE = activity.ACT_CDE
END
GO
