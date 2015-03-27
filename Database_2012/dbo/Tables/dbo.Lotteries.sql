CREATE TABLE [dbo].[Lotteries] (
    [Id]               INT           IDENTITY (1, 1) NOT NULL,
    [TimeStamp]        DATETIME      CONSTRAINT [DF_Lotteries_TimeStamp] DEFAULT (getdate()) NULL,
    [Name]             NVARCHAR (50) NULL,
    [Description]      NVARCHAR (50) NULL,
    [PrizeDescription] NVARCHAR (50) NULL,
    [PrizeID]          INT           NULL,
    [CreatedBy]        NVARCHAR (50) NULL,
    [TopPapEarner]     NVARCHAR (50) NULL,
    [Active]           BIT           NULL,
    CONSTRAINT [PK_Lotteries] PRIMARY KEY CLUSTERED ([Id] ASC)
);

