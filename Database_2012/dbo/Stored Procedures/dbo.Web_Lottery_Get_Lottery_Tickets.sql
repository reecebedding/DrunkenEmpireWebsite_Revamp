
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Web_Lottery_Get_Lottery_Tickets]
	@lotteryID int
AS
BEGIN
		SELECT * FROM [dbo].[LotteryTickets]
		WHERE LotteryID = @lotteryID
END

