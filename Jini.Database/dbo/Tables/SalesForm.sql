CREATE TABLE [dbo].[SalesForm] (
    [Id]               INT IDENTITY (1, 1) NOT NULL,
    [RefSalesFormCode] INT NOT NULL,
    CONSTRAINT [PK_SalesConfiguration_SalesForm] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SalesForm_RefSalesForm] FOREIGN KEY ([RefSalesFormCode]) REFERENCES [dbo].[RefSalesForm] ([Code])
);

















