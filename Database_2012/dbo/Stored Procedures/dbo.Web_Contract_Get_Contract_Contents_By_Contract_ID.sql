-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Web_Contract_Get_Contract_Contents_By_Contract_ID]
	@contractID int
AS
BEGIN
	SELECT * FROM [dbo].[ContractContents] WHERE ContractID = @contractID
END

