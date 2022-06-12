CREATE TABLE [dbo].[SalesConfigurationHistory] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [CreatedDate] DATETIME       NOT NULL,
    [Value]       NVARCHAR (MAX) NOT NULL,
    [CreatedBy]   NVARCHAR (50)  NOT NULL,
    [VersionNo] INT NOT NULL, 
    [Isbn] NCHAR(50) NOT NULL, 
    CONSTRAINT [PK_SalesConfigurationHistory] PRIMARY KEY CLUSTERED ([Id] ASC)
);



