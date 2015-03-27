-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Web_Api_Get_Api_By_ID]
	@ID int
AS
BEGIN
	SELECT * FROM ApiKeys
	WHERE Id = @ID
END

