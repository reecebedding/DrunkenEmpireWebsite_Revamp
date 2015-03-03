CREATE TABLE [dbo].[RecruitApplications] (
    [Id]                  BIGINT        IDENTITY (1, 1) NOT NULL,
    [ApplicantName]       VARCHAR (MAX) NOT NULL,
    [P1TimeStamp]         DATETIME      NOT NULL,
    [P1Recruiter]         VARCHAR (MAX) NULL,
    [P2TimeStamp]         DATETIME      NULL,
    [P2Recruiter]         VARCHAR (MAX) NULL,
    [CompletionTimeStamp] DATETIME      NULL,
    [CompletedBy]         VARCHAR (MAX) NULL,
    [Notes]               VARCHAR (MAX) NULL,
    [Status]              VARCHAR (MAX) NOT NULL,
    [Active]              BIT           CONSTRAINT [DF_RecruitApplications_Completed] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK__RecruitA__3214EC07412EB0B6] PRIMARY KEY CLUSTERED ([Id] ASC)
);

