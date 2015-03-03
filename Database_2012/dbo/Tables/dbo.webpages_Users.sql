CREATE TABLE [dbo].[webpages_Users] (
    [UserID]   INT           IDENTITY (1, 1) NOT NULL,
    [UserName] NVARCHAR (56) NOT NULL,
    [Email]    VARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([UserID] ASC),
    UNIQUE NONCLUSTERED ([UserName] ASC)
);

