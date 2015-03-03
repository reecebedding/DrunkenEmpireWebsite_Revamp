CREATE TABLE [dbo].[invTypes] (
    [typeID]              INT             NOT NULL,
    [groupID]             INT             NULL,
    [typeName]            NVARCHAR (100)  NULL,
    [description]         NVARCHAR (3000) NULL,
    [mass]                FLOAT (53)      NULL,
    [volume]              FLOAT (53)      NULL,
    [capacity]            FLOAT (53)      NULL,
    [portionSize]         INT             NULL,
    [raceID]              TINYINT         NULL,
    [basePrice]           MONEY           NULL,
    [published]           BIT             NULL,
    [marketGroupID]       INT             NULL,
    [chanceOfDuplicating] FLOAT (53)      NULL,
    CONSTRAINT [invTypes_PK] PRIMARY KEY CLUSTERED ([typeID] ASC)
);

