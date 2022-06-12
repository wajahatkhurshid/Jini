CREATE TABLE [dbo].[Seller] (
    [Id]   INT           IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (50) NOT NULL,
    [WebShopId] INT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_Seller] PRIMARY KEY CLUSTERED ([Id] ASC)
);

