-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Web_RecruitApplication_Get_Ship_Fitting_By_ID]
	@FittingID int
AS
BEGIN
	SELECT * FROM RecruitApplicationShipFittings
	WHERE ID = @FittingID
END

