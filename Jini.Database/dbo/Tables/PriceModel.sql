CREATE TABLE [dbo].[PriceModel] (
    [id]                INT           IDENTITY (1, 1) NOT NULL,
    [AccessFormId]      INT           NOT NULL,
    [RefPriceModelCode] INT           NOT NULL,
    [PercentValue]      INT           NULL,
    [GradeLevels]       NVARCHAR (50) NULL,
    CONSTRAINT [PK_AccessPriceModel] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_PriceModel_AccessForm] FOREIGN KEY ([AccessFormId]) REFERENCES [dbo].[AccessForm] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PriceModel_RefPriceModel] FOREIGN KEY ([RefPriceModelCode]) REFERENCES [dbo].[RefPriceModel] ([Code])
);











