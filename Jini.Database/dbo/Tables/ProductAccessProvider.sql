CREATE TABLE [dbo].[ProductAccessProvider] (
    [Id]               INT IDENTITY (1, 1) NOT NULL,
    [AccessProviderId] INT NOT NULL,
    [ProductId]        INT NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AccessProvider] FOREIGN KEY ([AccessProviderId]) REFERENCES [dbo].[RefAccessProvider] ([Code]),
    CONSTRAINT [FK_Product] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product] ([Id])
);

