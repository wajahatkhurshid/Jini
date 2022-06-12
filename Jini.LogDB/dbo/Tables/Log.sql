CREATE TABLE [dbo].[Log] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Level]       NVARCHAR (50)  CONSTRAINT [DF_Log_Level] DEFAULT (N'INFO') NOT NULL,
    [Application] NVARCHAR (800) NOT NULL,
    [MethodInfo]  NVARCHAR (800) NULL,
    [Message]     NVARCHAR (MAX) NOT NULL,
    [Exception]   NVARCHAR (MAX) NULL,
    [TimeStamp]   DATETIME       CONSTRAINT [DF__Log__TimeStamp__014935CB] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'This table stores log information of Ekey and Gyldendal Access Services.', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Log';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Will contains error level (like INFO, ERROR, CRITICALERROR).', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Log', @level2type = N'COLUMN', @level2name = N'Level';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'String containing the name of the Application creating the entry. Examples: Ekey, Gyldendal Access Services, Login Connector.', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Log', @level2type = N'COLUMN', @level2name = N'Application';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'String containing the name of the Subcomponent creating the entry. This can be a Controller, Batch Job etc.', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Log', @level2type = N'COLUMN', @level2name = N'MethodInfo';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Readable information about this log entry.', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Log', @level2type = N'COLUMN', @level2name = N'Message';

