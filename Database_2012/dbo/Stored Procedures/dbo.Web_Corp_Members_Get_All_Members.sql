CREATE PROCEDURE [dbo].[Web_Corp_Members_Get_All_Members]
AS
	SELECT * FROM CorpMembers
	ORDER BY PilotName ASC
RETURN 0
