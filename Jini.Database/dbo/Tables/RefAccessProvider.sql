CREATE TABLE [dbo].[RefAccessProvider] (
    [Code]        INT           NOT NULL,
    [Identifier]  VARCHAR (100) NOT NULL,
    [DisplayName] VARCHAR (100) NULL,
    PRIMARY KEY CLUSTERED ([Code] ASC)
);

