-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Web_RecruitApplication_Add_Application_Question]
	@AppID bigint,
	@QuestionID bigint,
	@Answer varchar(max)
AS
BEGIN
	
	INSERT INTO RecruitApplicationQuestionAnswers(ApplicationID, QuestionID, Answer)
	VALUES (@AppID, @QuestionID, @Answer)

END

