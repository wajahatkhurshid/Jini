CREATE TABLE [dbo].[DbVersion] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [VersionNo]   VARCHAR (50)  NOT NULL,
    [SequenceNo]  INT           NOT NULL,
    [CreatedDate] DATETIME      NOT NULL,
    [Note]        VARCHAR (250) NULL,
    [ScriptName]  VARCHAR (250) NOT NULL,
    CONSTRAINT [PK_DbVersion] PRIMARY KEY CLUSTERED ([Id] ASC)
);

