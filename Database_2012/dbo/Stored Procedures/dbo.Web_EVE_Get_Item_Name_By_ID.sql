-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Web_EVE_Get_Item_Name_By_ID] 
	@itemID bigint
AS
BEGIN
	SELECT typeName FROM [dbo].[invTypes]
	WHERE [typeID] = @itemID
END

