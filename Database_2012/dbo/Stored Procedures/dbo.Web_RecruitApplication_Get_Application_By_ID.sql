CREATE PROCEDURE [dbo].[Web_RecruitApplication_Get_Application_By_ID]
	@AppID bigint
AS
	SELECT * FROM RecruitApplications WHERE Id = @AppID
RETURN 0

