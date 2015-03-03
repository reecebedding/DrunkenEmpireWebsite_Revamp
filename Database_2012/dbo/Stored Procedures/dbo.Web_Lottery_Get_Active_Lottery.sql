-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE dbo.Web_Lottery_Get_Active_Lottery

AS
BEGIN
		SELECT TOP 1 *
		FROM Lotteries
		WHERE Active = 1
END
