
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Web_Contract_Create_New_ContractContent] 
	@ContractID int,
	@ItemID int,
	@PricePerUnit decimal(18, 0),
	@Quantity int	
AS
BEGIN
	BEGIN TRANSACTION InsertVals
	
	INSERT INTO [dbo].[ContractContents](ContractID, ItemID, ItemName, PricePerUnit, Quantity)
	VALUES(@ContractID, @ItemID, (SELECT typeName FROM [dbo].[invTypes] WHERE typeID = @ItemID), @PricePerUnit, @Quantity)

	COMMIT TRANSACTION InsertVals
END


