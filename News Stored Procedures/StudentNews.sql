SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StudentNews](
	[SNID] [int] IDENTITY(1,1) NOT NULL,
	[ADUN] [varchar](50) NOT NULL,
	[categoryID] [int] NOT NULL,
	[Subject] [varchar](60) NOT NULL,
	[Body] [varchar](6000) NOT NULL,
	[Accepted] [bit] NULL,
	[Sent] [bit] NULL,
	[thisPastMailing] [bit] NULL,
	[Entered] [datetime] NULL,
	[fname] [varchar](30) NULL,
	[lname] [varchar](30) NULL,
	[ManualExpirationDate] [datetime] NULL,
	[Image] [varchar](100) NULL,
 CONSTRAINT [PK_StudentNews] PRIMARY KEY CLUSTERED 
(
	[SNID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[StudentNews]  WITH CHECK ADD  CONSTRAINT [FK_StudentNews_StudentNewsCategory] FOREIGN KEY([categoryID])
REFERENCES [dbo].[StudentNewsCategory] ([categoryID])
GO
ALTER TABLE [dbo].[StudentNews] CHECK CONSTRAINT [FK_StudentNews_StudentNewsCategory]
GO
