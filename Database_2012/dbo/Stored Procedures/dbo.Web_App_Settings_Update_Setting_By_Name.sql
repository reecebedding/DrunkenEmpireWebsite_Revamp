CREATE PROCEDURE [dbo].[Web_App_Settings_Update_Setting_By_Name]
	@settingName varchar(max),
	@value varchar(max)
AS
	BEGIN TRANSACTION UpdateKeys
	
			UPDATE AppSettings
			SET Value = @value
			WHERE Setting = @settingName

			SELECT (1)

	COMMIT TRANSACTION UpdateKeys
RETURN 0

