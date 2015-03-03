CREATE TABLE [dbo].[ApiKeys] (
    [Id]        INT           IDENTITY (1, 1) NOT NULL,
    [PilotName] VARCHAR (50)  NOT NULL,
    [KeyID]     BIGINT        NOT NULL,
    [VCode]     VARCHAR (255) NOT NULL,
    [Valid]     BIT           NULL,
    [Errors]    VARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

