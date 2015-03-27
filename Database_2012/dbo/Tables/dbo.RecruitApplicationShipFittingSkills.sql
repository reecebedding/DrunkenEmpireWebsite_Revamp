CREATE TABLE [dbo].[RecruitApplicationShipFittingSkills] (
    [ID]            INT          IDENTITY (1, 1) NOT NULL,
    [FittingID]     INT          NOT NULL,
    [ShipPreReq]    BIT          NOT NULL,
    [SkillID]       INT          NOT NULL,
    [SkillName]     VARCHAR (50) NOT NULL,
    [RequiredLevel] INT          NOT NULL,
    CONSTRAINT [PK_RecruitApplicationShipFittingSkills] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_RecruitApplicationShipFittingSkills_RecruitApplicationShipFittings] FOREIGN KEY ([FittingID]) REFERENCES [dbo].[RecruitApplicationShipFittings] ([ID])
);

