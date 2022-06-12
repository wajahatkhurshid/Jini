CREATE TABLE [dbo].[AccessForm] (
    [Id]                   INT            IDENTITY (1, 1) NOT NULL,
    [SalesConfigurationId] INT            NOT NULL,
    [RefCode]              INT            NOT NULL,
    [WebText]              NVARCHAR (500) NULL,
    [DescriptionText]      NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_SalesConfiguration_AccessForm] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AccessForm_RefAccessForm] FOREIGN KEY ([RefCode]) REFERENCES [dbo].[RefAccessForm] ([Code]),
    CONSTRAINT [FK_AccessForm_SalesConfiguration] FOREIGN KEY ([SalesConfigurationId]) REFERENCES [dbo].[SalesConfiguration] ([Id]) ON DELETE CASCADE
);


























GO
CREATE NONCLUSTERED INDEX [_dta_index_AccessForm_17]
    ON [dbo].[AccessForm]([SalesConfigurationId] ASC, [Id] ASC, [RefCode] ASC)
    INCLUDE([WebText], [DescriptionText]);

