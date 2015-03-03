CREATE PROCEDURE [dbo].[Web_RecruitApplication_Add_Question]
	@description varchar(max),
	@dataType varchar(max),
	@answer bit
AS
	BEGIN TRANSACTION AddQuestion
		INSERT INTO RecruitApplicationQuestions([Description], DataType, Active)
		VALUES
		(@description, @dataType, @answer)
	COMMIT TRANSACTION AddQuestion
		SELECT (1)
RETURN 0
