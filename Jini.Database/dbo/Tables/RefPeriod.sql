CREATE TABLE [dbo].[RefPeriod] (
    [Id]                  INT IDENTITY (1, 1) NOT NULL,
    [UnitValue]           INT NOT NULL,
    [RefPeriodUnitTypeCode] INT NOT NULL,
    CONSTRAINT [PK_Period] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_RefPeriod_RefPeriodUnitType] FOREIGN KEY ([RefPeriodUnitTypeCode]) REFERENCES [dbo].[RefPeriodUnitType] ([Code])
);





