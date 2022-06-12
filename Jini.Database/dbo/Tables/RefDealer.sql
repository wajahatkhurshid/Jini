CREATE TABLE [dbo].[RefDealer] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [Name]            NVARCHAR (50)  NULL,
    [Text]            NVARCHAR (500) NULL,
    [LogoImage]       NVARCHAR (10)     NULL,
    [RefDealerTextId] INT            NOT NULL,
    CONSTRAINT [PK_Dealer] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_RefDealer_RefDealerText] FOREIGN KEY ([RefDealerTextId]) REFERENCES [dbo].[RefDealerText] ([Id])
);

