SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Cameron Abbot
-- Create date: 06/26/2020
-- Description:	Fetch all news
-- submitted by a particular user 
-- that has not yet been approved
-- and is unexpired
--	(CONVERT is used twice,
--	first to remove hour-minute-seconds, and
--	second to add hh:mm:ss.mmm all back as zero)
-- =============================================

ALTER PROCEDURE [dbo].[NEWS_PERSONAL_UNAPPROVED] 
	-- Add the parameters for the stored procedure here
	@Username varchar(50)

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
	WHERE Accepted = 0
	AND ADUN = @Username
	-- only delete unexpired news
	AND ((ManualExpirationDate IS NULL
	-- "and if the date 2 weeks from entry >= today's date"
	AND DATEADD(week, 2, CONVERT(datetime, CONVERT(date, Entered))) >= CONVERT(datetime, CONVERT(date, GETDATE())))
	OR (ManualExpirationDate IS NOT NULL
	AND ManualExpirationDate >= CONVERT(datetime, CONVERT(date, GETDATE()))))
	-- ORDER BY snc.SortOrder
	ORDER BY SNID desc

END
GO
