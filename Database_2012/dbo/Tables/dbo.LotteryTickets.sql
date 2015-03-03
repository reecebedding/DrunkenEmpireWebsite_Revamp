CREATE TABLE [dbo].[LotteryTickets] (
    [ID]             INT           IDENTITY (1, 1) NOT NULL,
    [LotteryID]      INT           NULL,
    [PilotName]      NVARCHAR (50) NULL,
    [PilotID]        INT           NULL,
    [TicketNumber]   INT           NULL,
    [WinnerPosition] INT           NULL,
    CONSTRAINT [PK_LotteryTickets] PRIMARY KEY CLUSTERED ([ID] ASC)
);

