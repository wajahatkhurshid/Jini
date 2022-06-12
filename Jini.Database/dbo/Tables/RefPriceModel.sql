CREATE TABLE [dbo].[RefPriceModel] (
    [Code]            INT            NOT NULL,
    [DisplayName]     NVARCHAR (50)  NOT NULL,
    [RefAccessFormCode] INT            NOT NULL,
    [Text]            NVARCHAR (500) NULL,
    [ShowPercentage]  BIT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_PriceModel] PRIMARY KEY CLUSTERED ([Code]),
    CONSTRAINT [FK_RefPriceModel_RefAccessForm] FOREIGN KEY ([RefAccessFormCode]) REFERENCES [dbo].[RefAccessForm] ([Code]),
    CONSTRAINT [Code_RefPriceModel] UNIQUE NONCLUSTERED ([Code] ASC)
);





