CREATE TABLE [dbo].[Product] (
    [Id]                INT           IDENTITY (1, 1) NOT NULL,
    [IsExternalLogin]   BIT           NOT NULL,
    [Isbn]              VARCHAR (13)  NOT NULL,
    [Title]             VARCHAR (300) NULL,
    [UnderTitle]        VARCHAR (300) NULL,
    [ReleaseDate]       DATETIME      NULL,
    [GradeLevels]       VARCHAR (300) NULL,
    [LastUpdatedDate]   DATETIME      NULL,
    [GpmMediaTypeId]    INT           NULL,
    [GpmMaterialTypeId] INT           NULL,
    [DepartmentCode]    VARCHAR (50)  NULL,
    [DepartmentName]    VARCHAR (50)  NULL,
    [SectionCode]       VARCHAR (50)  NULL,
    [SectionName]       VARCHAR (50)  NULL,
    [ContainerInstanceId] BIGINT NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [fk_GpmMaterialTypeId] FOREIGN KEY ([GpmMaterialTypeId]) REFERENCES [dbo].[MaterialTypes] ([GpmId]),
    CONSTRAINT [fk_GpmMediaTypeId] FOREIGN KEY ([GpmMediaTypeId]) REFERENCES [dbo].[MediaType] ([GpmId])
);













