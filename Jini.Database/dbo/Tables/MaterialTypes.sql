CREATE TABLE [dbo].[MaterialTypes] (
    [GpmId]          INT           NOT NULL,
    [Name]           VARCHAR (300) NOT NULL,
    [GpmMediaTypeId] INT           NOT NULL,
    PRIMARY KEY CLUSTERED ([GpmId] ASC),
    CONSTRAINT [fk_MediaType] FOREIGN KEY ([GpmMediaTypeId]) REFERENCES [dbo].[MediaType] ([GpmId])
);







