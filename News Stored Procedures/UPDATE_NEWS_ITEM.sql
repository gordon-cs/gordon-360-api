SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		Cameron Abbot
-- Create date: 06/30/2020
-- Description:	Update student news item
-- NOT CURRENTLY USED
-- =============================================

ALTER PROCEDURE [dbo].[UPDATE_NEWS_ITEM] 
	-- Add the parameters for the stored procedure here
	@SNID int,
	@Username varchar(50),
	@CategoryID int,
	@Subject varchar(60),
	@Body varchar(6000),
    @Image varchar(100)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Update student news item
	UPDATE MyGordon.dbo.StudentNews
	SET  categoryID = @CategoryID,
		 Subject = @Subject,
		 Body = @Body,
         Image = @Image,
		 Accepted = 0,
		 Sent = 0,
		 thisPastMailing = 0,
		 Entered = GETDATE()
	WHERE ADUN = @Username
	AND SNID = @SNID
	AND ((ManualExpirationDate IS NULL
	AND DATEADD(week, 2, CONVERT(datetime, CONVERT(date, Entered))) >= CONVERT(datetime, CONVERT(date, GETDATE())))
	OR (ManualExpirationDate IS NOT NULL
	AND ManualExpirationDate >= CONVERT(datetime, CONVERT(date, GETDATE()))))

END
GO
