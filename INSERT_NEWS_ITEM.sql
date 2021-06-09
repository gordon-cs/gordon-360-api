SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Cameron Abbot
-- Create date: 6/26/2020
-- Description:	Inserts News Item for specific ID
-- ===============================s==============
ALTER PROCEDURE [dbo].[INSERT_NEWS_ITEM] 
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

    -- Insert statements for procedure here
	INSERT INTO MyGordon.dbo.StudentNews (ADUN, categoryID, Subject, Body, Image, Accepted, Sent, thisPastMailing, Entered)
	VALUES (@Username, @CategoryID, @Subject, @Body, @Image,0, 1, 0, GETDATE())
END

GO
