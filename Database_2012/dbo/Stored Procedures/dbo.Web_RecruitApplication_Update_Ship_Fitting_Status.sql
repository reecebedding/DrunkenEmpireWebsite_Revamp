-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Web_RecruitApplication_Update_Ship_Fitting_Status]
	@fittingID int,
	@status bit
AS
BEGIN
	UPDATE RecruitApplicationShipFittings
	SET Active = @status
	WHERE ID = @fittingID
END

