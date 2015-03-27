CREATE TABLE [dbo].[CorpMembers] (
    [PilotID]   BIGINT        NOT NULL,
    [PilotName] VARCHAR (MAX) NOT NULL,
    [LastLogon] DATETIME      NULL,
    [Location]  VARCHAR (MAX) NULL,
    [Ship]      VARCHAR (MAX) NULL,
    [Roles]     BIGINT        NULL,
    CONSTRAINT [PK_CorpRoster] PRIMARY KEY CLUSTERED ([PilotID] ASC)
);

