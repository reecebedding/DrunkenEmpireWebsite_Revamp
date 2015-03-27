CREATE TABLE [dbo].[AltList] (
    [Id]         INT           IDENTITY (1, 1) NOT NULL,
    [MainName]   VARCHAR (MAX) NOT NULL,
    [AltName]    VARCHAR (MAX) NOT NULL,
    [AltPurpose] VARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

