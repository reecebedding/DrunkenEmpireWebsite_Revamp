CREATE TABLE [dbo].[RecruitApplicationQuestionAnswers] (
    [UID]           BIGINT        IDENTITY (1, 1) NOT NULL,
    [ApplicationID] BIGINT        NOT NULL,
    [QuestionID]    BIGINT        NOT NULL,
    [Answer]        VARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_RecruitApplicationQuestionAnswers] PRIMARY KEY CLUSTERED ([UID] ASC),
    CONSTRAINT [FK_RecruitApplicationQuestionAnswers_RecruitApplicationQuestions] FOREIGN KEY ([QuestionID]) REFERENCES [dbo].[RecruitApplicationQuestions] ([Id]),
    CONSTRAINT [FK_RecruitApplicationQuestionAnswers_RecruitApplications] FOREIGN KEY ([ApplicationID]) REFERENCES [dbo].[RecruitApplications] ([Id]),
    CONSTRAINT [FK_RecruitApplicationQuestionAnswers_ToRecruitApplicationQuestions] FOREIGN KEY ([QuestionID]) REFERENCES [dbo].[RecruitApplicationQuestions] ([Id]),
    CONSTRAINT [FK_RecruitApplicationQuestionAnswers_ToRecruitApplications] FOREIGN KEY ([ApplicationID]) REFERENCES [dbo].[RecruitApplications] ([Id])
);

