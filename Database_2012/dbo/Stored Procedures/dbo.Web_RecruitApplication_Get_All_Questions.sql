CREATE PROCEDURE [dbo].[Web_RecruitApplication_Get_All_Questions]
	
AS
	SELECT Id, [Description], DataType, Active FROM RecruitApplicationQuestions
RETURN 0
