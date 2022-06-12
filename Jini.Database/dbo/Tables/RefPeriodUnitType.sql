CREATE TABLE [dbo].[RefPeriodUnitType] (
    [Code]        INT           NOT NULL,
    [DisplayName] NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_PeriodDurationType] PRIMARY KEY CLUSTERED ([Code]),
    CONSTRAINT [Code_RefPeriodUnitType] UNIQUE NONCLUSTERED ([Code] ASC)
);



