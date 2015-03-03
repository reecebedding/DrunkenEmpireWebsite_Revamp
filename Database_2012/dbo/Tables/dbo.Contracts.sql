CREATE TABLE [dbo].[Contracts] (
    [UID]          INT           IDENTITY (1, 1) NOT NULL,
    [Creator]      VARCHAR (255) NOT NULL,
    [Created]      DATETIME      NOT NULL,
    [Total]        DECIMAL (18)  NOT NULL,
    [Status]       VARCHAR (50)  NOT NULL,
    [RejectReason] VARCHAR (255) NULL,
    [ProcessedBy]  VARCHAR (50)  NULL,
    CONSTRAINT [PK_Contract] PRIMARY KEY CLUSTERED ([UID] ASC),
    CONSTRAINT [FK_Contracts_Contracts] FOREIGN KEY ([UID]) REFERENCES [dbo].[Contracts] ([UID])
);

