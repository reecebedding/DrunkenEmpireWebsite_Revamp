-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Web_RecruitApplication_Update_Application_Background_Check]
	@appID bigint, 
	@notes varchar(max) = null,
	@action varchar(max),
	@activeUser varchar(max)
AS
BEGIN
	UPDATE RecruitApplications
	SET Notes = @notes,
	Status = @action,
	P2Recruiter = @activeUser,
	P2TimeStamp = GETDATE()
	WHERE Id = @appID
END

