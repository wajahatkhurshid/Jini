CREATE TABLE [dbo].[RefSeller] (
    [Id]   INT          IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_RefSeller] PRIMARY KEY CLUSTERED ([Id] ASC)
);

