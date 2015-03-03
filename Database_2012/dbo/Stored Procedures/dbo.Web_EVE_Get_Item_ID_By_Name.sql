-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Web_EVE_Get_Item_ID_By_Name] 
	@itemName varchar(255)
AS
BEGIN
	SELECT typeID FROM [dbo].[invTypes]
	WHERE typeName = @itemName
END

