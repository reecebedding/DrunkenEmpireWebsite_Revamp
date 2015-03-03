CREATE PROCEDURE [dbo].[Web_App_Settings_Update_Director_API]
	@keyID VARCHAR(MAX),
	@vCode VARCHAR(MAX)
AS
	IF (((SELECT COUNT(*) FROM AppSettings WHERE Setting = 'DirectorKeyID' AND Value = @keyID) = 0) AND ((SELECT COUNT(*) FROM AppSettings WHERE Setting = 'DirectorVCode' AND Value = @vCode) = 0 ))
	BEGIN
		BEGIN TRANSACTION UpdateKeys
	
			UPDATE AppSettings
			SET Value = @keyID
			WHERE Setting = 'DirectorKeyID'

			UPDATE AppSettings
			SET Value = @vCode
			WHERE Setting = 'DirectorVCode'

			SELECT (1)

		COMMIT TRANSACTION UpdateKeys
	END
	ELSE
	BEGIN
		SELECT (2)		
	END
RETURN 0
