CREATE PROCEDURE [dbo].[Web_Api_Get_All_Apis_For_Pilot]
	@pilotName varchar(255)
AS

	DECLARE @establishedMain VARCHAR(MAX)
	SET @establishedMain = @pilotName

	IF ((SELECT COUNT(*) FROM AltList WHERE MainName = @pilotName) > 0)
		SET @establishedMain = @pilotName
	IF ((SELECT COUNT(*) FROM AltList WHERE AltName = @pilotName) > 0)
		SET @establishedMain = (SELECT MainName FROM AltList WHERE AltName = @pilotName)
	
	SELECT * FROM ApiKeys
	WHERE PilotName = @establishedMain OR
	PilotName IN (SELECT AltName FROM AltList WHERE MainName = @establishedMain)

RETURN 0
