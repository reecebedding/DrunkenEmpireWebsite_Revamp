--Description: Inserts key to table if the pilot doesnt allready have a key assoicated, returns 1. If key does exists, does not update / insert, returns 2. Return 0 if fails.
CREATE PROCEDURE [dbo].[Web_Api_Add_Api]
	@KeyID bigint,
	@VCode varchar(255),
	@PilotName varchar(50)
AS
	IF NOT EXISTS
	(SELECT * FROM ApiKeys WHERE PilotName = @PilotName)
	BEGIN
		INSERT INTO ApiKeys (PilotName, KeyID, VCode, Valid)
		VALUES (@PilotName, @KeyID, @VCode, 'true')
		SELECT 1
	END
	ELSE
	BEGIN
		SELECT 2
	END
SELECT 0
