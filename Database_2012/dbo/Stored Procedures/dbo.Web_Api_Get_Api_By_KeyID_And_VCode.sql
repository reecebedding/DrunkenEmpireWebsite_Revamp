CREATE PROCEDURE [dbo].[Web_Api_Get_Api_By_KeyID_And_VCode]
	@keyID bigint,
	@vCode varchar(max)
AS
	SELECT * FROM ApiKeys 
	WHERE KeyID = @keyID AND VCode = @vCode
RETURN 0
