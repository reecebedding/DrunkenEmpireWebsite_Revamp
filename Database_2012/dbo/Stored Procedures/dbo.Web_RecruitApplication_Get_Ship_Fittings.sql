-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Web_RecruitApplication_Get_Ship_Fittings]
	@ActiveOnly bit
	AS
BEGIN
	
	DECLARE @SQL nvarchar(max)
	SET @SQL = 'SELECT * FROM RecruitApplicationShipFittings'
	IF @ActiveOnly = 1
	BEGIN
		SET @SQL = @SQL + ' WHERE Active = @ActiveOnly'
	END
	EXECUTE sp_executesql @SQL, N'@ActiveOnly bit', @ActiveOnly = @ActiveOnly
END

