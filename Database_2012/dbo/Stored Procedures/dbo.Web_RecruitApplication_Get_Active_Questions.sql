CREATE PROCEDURE [dbo].[Web_RecruitApplication_Get_Active_Questions]
	
AS
	SELECT Id, Description, DataType, Active FROM RecruitApplicationQuestions
	WHERE Active = 'true';
RETURN 0
