-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Web_RecruitApplication_Remove_Ship_Fitting]
	@fittingID int
AS
BEGIN
	DELETE FROM RecruitApplicationShipFittings
	WHERE ID = @fittingID
END

