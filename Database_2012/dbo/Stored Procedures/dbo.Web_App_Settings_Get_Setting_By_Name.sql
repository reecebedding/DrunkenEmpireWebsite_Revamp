CREATE PROCEDURE [dbo].[Web_App_Settings_Get_Setting_By_Name]
	@settingName varchar(max)
AS
	SELECT * FROM AppSettings
	WHERE Setting = @settingName
RETURN 0
