USE [YOUR_DATABASE_NAME]

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE [TABLE_SCHEMA] = 'dbo' AND [TABLE_NAME] = 'QueryLogs')

BEGIN
	CREATE TABLE [dbo].[QueryLogs] (
		[ID] [int] IDENTITY(1,1) NOT NULL,
		[QueryID] [uniqueidentifier] NOT NULL,
		[ApplicationName] [nvarchar](50) NOT NULL,
		[QueryCategory] [nvarchar](100) NULL,
		[QueryTerms] [nvarchar](max) NULL,
		[QueryText] [nvarchar](max) NULL,
		[Host] [nvarchar](100) NULL,
		[IPAddress] [varchar](40) NULL,
		[ExecutedSuccessfully] [bit] NOT NULL,
		[ExecutionTime] [int] NULL,
		[ResultCount] [int] NULL,
		[LogDate] [datetime] NULL,
		[JsonData] [nvarchar](max) NULL,
	 CONSTRAINT [PK_QueryLogs] PRIMARY KEY CLUSTERED ([ID] ASC)
	 WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END