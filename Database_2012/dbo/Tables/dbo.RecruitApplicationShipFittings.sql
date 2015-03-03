CREATE TABLE [dbo].[RecruitApplicationShipFittings] (
    [ID]          INT           IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (50)  NOT NULL,
    [Description] VARCHAR (255) NULL,
    [Active]      BIT           NOT NULL,
    [ShipType]    VARCHAR (50)  NOT NULL,
    [XMLData]     XML           NOT NULL,
    CONSTRAINT [PK_RecruitApplicationShipFittings] PRIMARY KEY CLUSTERED ([ID] ASC)
);

