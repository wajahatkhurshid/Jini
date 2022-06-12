CREATE TABLE [dbo].[PeriodPrice] (
    [Id]             INT           IDENTITY (1, 1) NOT NULL,
    [UnitValue]      INT           NOT NULL,
    [RefPeriodTypeCode]       INT           NOT NULL,
    [AccessFormId]   INT           NOT NULL,
    [Currency]       NVARCHAR (10) NOT NULL,
    [UnitPrice]      MONEY         NOT NULL,
    [UnitPriceVat]   MONEY         NOT NULL,
    [VatValue]       INT           NOT NULL,
    [IsCustomPeriod] BIT           CONSTRAINT [DF__PeriodPri__IsCus__35BCFE0A] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_SalesConfigurationPeriod] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_PeriodPrice_RefPeriodUnitType] FOREIGN KEY ([RefPeriodTypeCode]) REFERENCES [dbo].[RefPeriodUnitType]([Code]),
    CONSTRAINT [FK_Period_AccessForm] FOREIGN KEY ([AccessFormId]) REFERENCES [dbo].[AccessForm] ([Id]) ON DELETE CASCADE
);





