CREATE TABLE [dbo].[SalesConfiguration] (
    [Id]                     INT           IDENTITY (1, 1) NOT NULL,
    [Isbn]                   NVARCHAR (13) NOT NULL,
    [SellerId]               INT           NOT NULL,
    [SalesChannel]           NVARCHAR (50) NOT NULL,
    [State]                  INT           NOT NULL,
    [SalesFormId]            INT           NULL,
    [CreatedDate]            DATETIME      NOT NULL,
    [RevisionNumber]         INT           NOT NULL,
    [CreatedBy]              NVARCHAR (50) NOT NULL,
    [TrialLicenseId]         INT           NULL,
    [RefSalesConfigTypeCode] INT           NULL,
    CONSTRAINT [PK_SalesConfiguration] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SalesConfiguration_RefSalesConfigType] FOREIGN KEY ([RefSalesConfigTypeCode]) REFERENCES [dbo].[RefSalesConfigType] ([Code]) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT [FK_SalesConfiguration_SalesForm] FOREIGN KEY ([SalesFormId]) REFERENCES [dbo].[SalesForm] ([Id]),
    CONSTRAINT [FK_SalesConfiguration_Seller] FOREIGN KEY ([SellerId]) REFERENCES [dbo].[Seller] ([Id]),
    CONSTRAINT [FK_SalesConfiguration_TrialLicense] FOREIGN KEY ([TrialLicenseId]) REFERENCES [dbo].[TrialLicense] ([Id]),
    CONSTRAINT [UC_SalesConfiguration] UNIQUE NONCLUSTERED ([Isbn] ASC, [SalesChannel] ASC, [State] ASC, [SalesFormId] ASC)
);











