-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE dbo.Web_Alt_Get_Alt
	@altName varchar(255)
AS
BEGIN
	SELECT * FROM [dbo].[AltList] 
	WHERE AltName = @altName
END
