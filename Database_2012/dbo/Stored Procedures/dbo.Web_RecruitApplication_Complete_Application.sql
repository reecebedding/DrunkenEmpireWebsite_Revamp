CREATE PROCEDURE [dbo].[Web_RecruitApplication_Complete_Application]
	@appID bigint, 
	@result varchar(max),
	@activeUser varchar(max)
AS
	UPDATE RecruitApplications
	SET CompletionTimeStamp = GETDATE(),
	CompletedBy = @activeUser,
	Status = @result,
	Active = 'false'
	WHERE Id = @appID

RETURN 0

