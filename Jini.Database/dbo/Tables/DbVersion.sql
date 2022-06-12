CREATE TABLE [dbo].[DbVersion] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [VersionNo]   NVARCHAR (50)  NOT NULL,
    [SequenceNo]  INT           NOT NULL,
    [CreatedDate] DATETIME      NOT NULL,
    [Note]        NVARCHAR (250) NULL,
    [ScriptName]  NVARCHAR (250) NOT NULL,
    CONSTRAINT [PK_DbVersion] PRIMARY KEY CLUSTERED ([Id] ASC)
);

