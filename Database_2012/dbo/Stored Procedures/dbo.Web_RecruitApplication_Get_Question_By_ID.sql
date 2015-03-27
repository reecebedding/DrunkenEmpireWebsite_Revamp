CREATE PROCEDURE [dbo].[Web_RecruitApplication_Get_Question_By_ID]
	@ID bigint
AS
	SELECT Id, Description, DataType, Active  FROM RecruitApplicationQuestions
	WHERE Id = @ID
RETURN 0
