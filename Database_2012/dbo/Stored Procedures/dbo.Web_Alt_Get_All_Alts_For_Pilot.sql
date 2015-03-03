CREATE PROCEDURE [dbo].[Web_Alt_Get_All_Alts_For_Pilot]
	@pilotName varchar(max)
AS
	IF ((SELECT COUNT(*) FROM AltList
		WHERE mainName = @pilotName) = 0)
	BEGIN
		IF((SELECT COUNT(*) FROM AltList WHERE altName = @pilotName) > 0)
		BEGIN
			SELECT * FROM AltList
			WHERE mainName = (SELECT DISTINCT mainName FROM AltList WHERE altName = @pilotName)
			ORDER BY altName
		END
	END
	ELSE
		SELECT * FROM AltList
		WHERE mainName = @pilotName
		ORDER BY altName
RETURN 0
