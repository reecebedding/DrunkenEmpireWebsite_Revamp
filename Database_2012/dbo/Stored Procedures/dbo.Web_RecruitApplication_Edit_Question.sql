CREATE PROCEDURE [dbo].[Web_RecruitApplication_Edit_Question]
	@Id int,
	@Description varchar(max),
	@DataType varchar(max),
	@Active bit
AS
	BEGIN TRANSACTION UpdateQuestion
		BEGIN TRY
			UPDATE RecruitApplicationQuestions SET
			[Description] = @Description,
			DataType = @DataType,
			Active = @Active
			WHERE Id = @Id

		    COMMIT TRANSACTION UpdateQuestion
			SELECT 1
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION UpdateQuestion
		SELECT 0
	END CATCH

	SELECT 2