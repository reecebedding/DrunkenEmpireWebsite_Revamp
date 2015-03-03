-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Web_RecruitApplication_Get_Applications_Base_By_Name]
	@ApplicantName varchar(max)
AS
BEGIN
	SELECT * FROM RecruitApplications
	WHERE ApplicantName = @ApplicantName
END

