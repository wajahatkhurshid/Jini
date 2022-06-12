CREATE TABLE [dbo].[RefSalesForm] (
    [Code]               INT           NOT NULL,
    [DisplayName]        NVARCHAR (50) NOT NULL,
    [ExternalIdentifier] INT           NOT NULL,
    [PeriodTypeName]     NVARCHAR (50) NULL,
    CONSTRAINT [PK_SalesForm] PRIMARY KEY CLUSTERED ([Code]),
    CONSTRAINT [Code_RefSalesForm] UNIQUE NONCLUSTERED ([Code] ASC)
);







