CREATE TABLE [dbo].[Logs] (
    [Id]            INT           IDENTITY (1, 1) NOT NULL,
    [TimeStamp]     VARCHAR (MAX) NULL,
    [IPAddress]     VARCHAR (MAX) NULL,
    [Level]         VARCHAR (MAX) NULL,
    [User]          VARCHAR (MAX) NULL,
    [CallSiteClass] VARCHAR (MAX) NULL,
    [StackTrace]    VARCHAR (MAX) NULL,
    [Message]       VARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

