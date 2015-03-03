CREATE PROCEDURE [dbo].[Web_EVE_Get_Primary_Skill_Requirements_For_Item]
	@itemID int = null,
	@itemName varchar(255) = null
AS

	IF @itemID IS NOT NULL
	BEGIN
		IF @itemName IS NULL
		BEGIN
			SET @itemName = (SELECT typeName FROM invTypes WHERE typeID = @itemID)
		END
	END
	ELSE
	BEGIN
		SET @itemID = (SELECT typeID FROM invTypes WHERE typeName = @itemName)
	END

	SELECT COALESCE(dgmTA.valueInt, dgmTA.valueFloat) AS 'SkillID',
	invT.typeName AS 'SkillName', 
	(COALESCE(
		(SELECT valueInt FROM dgmTypeAttributes WHERE typeID = @itemID AND attributeID = (CASE dgmTA.attributeID WHEN 182 THEN 277 WHEN 183 THEN 278 WHEN 184 THEN 279 WHEN 1286 THEN  1285 END))
		, 
		(SELECT valueFloat FROM dgmTypeAttributes WHERE typeID = @itemID AND attributeID = (CASE dgmTA.attributeID WHEN 182 THEN 277 WHEN 183 THEN 278 WHEN 184 THEN 279 WHEN 1285 THEN 1286 WHEN 1289 THEN 1287 WHEN 1290 THEN 1288 END))
	)) AS 'RequiredLevel' --, '' , *
	FROM 
	dgmTypeAttributes dgmTA
	JOIN dgmAttributeTypes dgmAT ON dgmAT.attributeID = dgmTA.attributeID
	LEFT JOIN invTypes invT ON invT.typeID = COAlESCE(dgmTA.valueInt, dgmTA.valueFloat)
	WHERE dgmTA.typeID = @itemID and dgmAT.categoryID = 8 and dgmAT.displayName IS NOT NULL and typeName IS NOT NULL
RETURN 0

