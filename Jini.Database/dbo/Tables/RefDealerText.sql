CREATE TABLE [dbo].[RefDealerText] (
    [Id]   INT            IDENTITY (1, 1) NOT NULL,
    [Text] NVARCHAR (500) NULL,
    CONSTRAINT [PK_DealerText] PRIMARY KEY CLUSTERED ([Id] ASC)
);

