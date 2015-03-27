CREATE PROCEDURE [dbo].[Web_RecruitApplication_Get_Application_Questions_By_AppID]
	@ApplicationID bigint
AS
	SELECT RAQ.Id, RAQ.Description, RAQA.Answer
	FROM RecruitApplicationQuestionAnswers RAQA
	JOIN RecruitApplicationQuestions RAQ ON RAQA.QuestionID = RAQ.Id
	WHERE ApplicationID = @ApplicationID
RETURN 0

