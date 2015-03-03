-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Web_Contract_Get_Contract_By_Status_And_Name]
	@status varchar(255),
	@userName varchar(255) = null
AS
BEGIN
	IF @userName IS NULL
	BEGIN
		SELECT * FROM [dbo].[Contracts] WHERE [Status] = @status
	END
	ELSE
	BEGIN
		SELECT * FROM [dbo].[Contracts] WHERE [Status] = @status AND Creator = @userName
	END
END

