CREATE PROCEDURE [dbo].[Web_Corp_Members_Add_Corp_Member]
	@pilotID bigint,
	@pilotName varchar(max),
	@lastLogon datetime,
	@location varchar(max),
	@ship varchar(max),
	@roles bigint
AS
	BEGIN TRANSACTION InsertCorpMember
		INSERT INTO CorpMembers (PilotID, PilotName, LastLogon, Location, Ship, Roles)
		VALUES
		(@pilotID, @pilotName, @lastLogon, @location, @ship, @roles)
	COMMIT TRANSACTION InsertCoprMember
		SELECT (1)
RETURN 0
