-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Web_RecruitApplication_Get_Applications_Base_By_Id]
	@ApplicationID bigint
AS
BEGIN
	SELECT * FROM RecruitApplications
	WHERE Id = @ApplicationID
END

