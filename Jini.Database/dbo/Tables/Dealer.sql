CREATE TABLE [dbo].[Dealer] (
    [Id]                   INT          NOT NULL,
    [Code]               INT NOT NULL,
    [Url]                  NVARCHAR (50) NULL,
    [DealerName]           NVARCHAR (50) NULL,
    [DealerType]           NVARCHAR (50) NULL,
    [SalesConfigurationId] INT          NOT NULL,
    CONSTRAINT [PK_Dealer_1] PRIMARY KEY CLUSTERED ([Id] ASC)
);



