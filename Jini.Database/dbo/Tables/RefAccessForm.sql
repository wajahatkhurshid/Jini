CREATE TABLE [dbo].[RefAccessForm] (
    [Code]               INT            NOT NULL,
    [DisplayName]        NVARCHAR (50)  NOT NULL,
    [WebText]            NVARCHAR (500) NULL,
    [ExternalIdentifier] INT            NULL,
    CONSTRAINT [PK_AccessForm] PRIMARY KEY CLUSTERED ([Code]),
    CONSTRAINT [Code_RefAccessForm] UNIQUE NONCLUSTERED ([Code] ASC) 
);





