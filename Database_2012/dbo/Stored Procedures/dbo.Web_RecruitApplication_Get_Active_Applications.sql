CREATE PROCEDURE [dbo].[Web_RecruitApplication_Get_Active_Applications]

AS
	SELECT * FROM RecruitApplications
	WHERE Active = 1
RETURN 0

