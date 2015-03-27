CREATE TABLE [dbo].[RecruitApplicationQuestions] (
    [Id]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [Description] NVARCHAR (MAX) NULL,
    [DataType]    NVARCHAR (MAX) NULL,
    [Active]      BIT            NULL,
    CONSTRAINT [PK__RecruitA__3214EC073D5E1FD2] PRIMARY KEY CLUSTERED ([Id] ASC)
);

