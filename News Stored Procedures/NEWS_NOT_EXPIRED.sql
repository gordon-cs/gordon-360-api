SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






-- =============================================
-- Author:		Josh Rogers
-- Create date: 05/13/2020
-- Description:	Fetch all information, 
--	except Accepted status, about all news 
--	not expired (CONVERT is used twice,
--	first to remove hour-minute-seconds, and
--	second to add hh:mm:ss.mmm all back as zero)
-- =============================================
-- Modified by: Josh Rogers
-- Date: 05/18/2020
-- Explanation: ManualExpirationDate was added as a
--  column to StudentNews
-- =============================================
-- Modified by: Josh Rogers
-- Date: 05/18/2020
-- Explanation: fname and lname are always null so
--  they have been removed from select
-- =============================================

ALTER PROCEDURE [dbo].[NEWS_NOT_EXPIRED] 
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT SNID, ADUN, sn.categoryID, Subject, Body, Image, Sent, thisPastMailing, Entered, categoryName, SortOrder, ManualExpirationDate
	FROM MyGordon.dbo.StudentNews sn 
	INNER JOIN MyGordon.dbo.StudentNewsCategory snc 
	ON sn.categoryID = snc.categoryID
	WHERE Accepted = 1 
	AND ((ManualExpirationDate IS NULL
	AND DATEADD(week, 2, CONVERT(datetime, CONVERT(date, Entered))) >= CONVERT(datetime, CONVERT(date, GETDATE())))
	OR (ManualExpirationDate IS NOT NULL
	AND ManualExpirationDate >= CONVERT(datetime, CONVERT(date, GETDATE()))))
	-- ORDER BY snc.SortOrder
	ORDER BY Entered desc

END
GO
