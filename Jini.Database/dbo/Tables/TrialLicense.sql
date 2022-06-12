
CREATE TABLE [dbo].[TrialLicense](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TrialPeriodId] [int] NULL,
	[MultipleTrials] [bit] NOT NULL,
	[TrialAccessFormCode] [int] NULL,
	[TrialCountId] [int] NULL,
	[ContactSalesText] [nvarchar](max) NULL,
 CONSTRAINT [PK__TrialLicense__3214EC076C44B463] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY], 
    CONSTRAINT [FK_TrialLicense_TrialCount] FOREIGN KEY ([TrialCountId]) REFERENCES [TrialCount]([Id]), 
    CONSTRAINT [FK_TrialLicense_RefTrialAccessForm] FOREIGN KEY ([TrialAccessFormCode]) REFERENCES [RefTrialAccessForm]([Code]), 
    CONSTRAINT [FK_TrialLicense_TrialPeriod] FOREIGN KEY ([TrialPeriodId]) REFERENCES [TrialPeriod]([Id])
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[TrialLicense] CHECK CONSTRAINT [FK_TrialLicense_TrialCount]
GO

ALTER TABLE [dbo].[TrialLicense] CHECK CONSTRAINT [FK_TrialLicense_TrialPeriod]
GO



