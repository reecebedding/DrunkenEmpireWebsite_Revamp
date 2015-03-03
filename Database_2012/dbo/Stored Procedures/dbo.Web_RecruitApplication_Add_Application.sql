-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Web_RecruitApplication_Add_Application]
	@ApplicantName varchar(max),	
	@P1Recruiter varchar(max) = null,
	@Notes varchar(max) = null
AS
BEGIN

	BEGIN TRANSACTION CreateApplication
		INSERT INTO RecruitApplications(ApplicantName, P1Recruiter, P1TimeStamp, Notes, [Status], Active) 
		VALUES (@ApplicantName, @P1Recruiter, GETDATE(),@Notes, 'P1', 'true')
	COMMIT TRANSACTION CreateApplication
		SELECT (@@IDENTITY)
RETURN 0
END

