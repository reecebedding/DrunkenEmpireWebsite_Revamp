CREATE PROCEDURE [dbo].[Web_RecruitApplication_Add_Ship_Fitting_Skill]
	@FittingID int,
	@SkillID int = NULL,
	@SkillName varchar(50) = NULL,
	@RequiredLevel int
AS
	IF (@SkillID IS NOT NULL)
	BEGIN
		SET @SkillName = (SELECT typeName FROM invTypes WHERE typeID = @SkillID)	
	END
	ELSE
	BEGIN
		IF @SkillName IS NOT NULL
		BEGIN
			SET @SkillID = (SELECT typeID FROM invTypes WHERE typeName = @SkillName)
		END
		ELSE
		BEGIN
			RETURN SELECT -1
		END
	END
	
	IF NOT EXISTS (SELECT * FROM RecruitApplicationShipFittingSkills WHERE FittingID = @FittingID AND SkillID = @SkillID)
	BEGIN
		BEGIN TRANSACTION CREATE_NEW_FITTING_SKILL
			INSERT INTO RecruitApplicationShipFittingSkills (FittingID, SkillID, SkillName, RequiredLevel)
			VALUES(@FittingID, @SkillID, @SkillName, @RequiredLevel)
		COMMIT TRANSACTION CREATE_NEW_FITTING_SKILL
		RETURN SELECT @@IDENTITY
	END
	ELSE
	BEGIN
		RETURN SELECT -1
	END
RETURN 0




