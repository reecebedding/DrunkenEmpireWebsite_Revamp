CREATE TABLE [dbo].[ContractContents] (
    [UID]          INT           IDENTITY (1, 1) NOT NULL,
    [ContractID]   INT           NOT NULL,
    [ItemID]       BIGINT        NOT NULL,
    [ItemName]     VARCHAR (255) NOT NULL,
    [PricePerUnit] DECIMAL (18)  NOT NULL,
    [Quantity]     INT           NOT NULL,
    CONSTRAINT [PK_ContractContents] PRIMARY KEY CLUSTERED ([UID] ASC),
    CONSTRAINT [FK_ContractContents_Contracts] FOREIGN KEY ([ContractID]) REFERENCES [dbo].[Contracts] ([UID])
);

