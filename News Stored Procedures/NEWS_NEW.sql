SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Josh Rogers
-- Create date: 05/13/2020
-- Description:	Fetch all information, 
--	except Accepted status, about all news 
--	that is new since 10 AM yesterday and not 
--  already expired (CONVERT is used twice,
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
--  they have been removed from select, and <
--  is more readable than >=
-- =============================================
-- Modified by: Hudson Finn
-- Date: 02/16/2021
-- Explanation: Changed the procdure to retrieve news
-- from 24hrs earlier rather than 10am the previous day
-- per issue #407


ALTER PROCEDURE [dbo].[NEWS_NEW] 
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
	AND GETDATE() - 1 < Entered
	AND (ManualExpirationDate IS NULL
	OR CONVERT(datetime, CONVERT(date, GETDATE())) < ManualExpirationDate)
	ORDER BY snc.SortOrder

END
GO
