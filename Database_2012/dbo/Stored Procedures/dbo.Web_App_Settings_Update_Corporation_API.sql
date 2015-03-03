CREATE PROCEDURE [dbo].[Web_App_Settings_Update_Corporation_API]
	@keyID VARCHAR(MAX),
	@vCode VARCHAR(MAX)
AS
	IF (((SELECT COUNT(*) FROM AppSettings WHERE Setting = 'CorporationKeyID' AND Value = @keyID) = 0) AND ((SELECT COUNT(*) FROM AppSettings WHERE Setting = 'CorporationKeyID' AND Value = @vCode) = 0 ))
	BEGIN
		BEGIN TRANSACTION UpdateKeys
	
			UPDATE AppSettings
			SET Value = @keyID
			WHERE Setting = 'CorporationKeyID'

			UPDATE AppSettings
			SET Value = @vCode
			WHERE Setting = 'CorporationVCode'

			SELECT (1)

		COMMIT TRANSACTION UpdateKeys
	END
	ELSE
	BEGIN
		SELECT (2)		
	END
RETURN 0
