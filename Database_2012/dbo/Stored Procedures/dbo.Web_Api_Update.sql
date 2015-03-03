CREATE PROCEDURE [dbo].[Web_Api_Update]
	@oldAPIKeyID bigint,
	@oldAPIVCode varchar(max),
	@newAPIKeyID bigint,
	@newAPIVCode varchar(max)
AS
BEGIN TRANSACTION UpdateTransaction;
	UPDATE ApiKeys SET KeyID = @newAPIKeyID, VCode = @newAPIVCode, Valid = 'true'
	WHERE
	KeyID = @oldAPIKeyID AND VCode = @oldAPIVCode
	SELECT (1)
COMMIT TRANSACTION UpdateTransaction;
RETURN 0
