CREATE PROCEDURE [dbo].[Web_RecruitApplication_Add_Ship_Fitting]
	@FittingName varchar(50),
	@FittingDescription varchar(255) = NULL,
	@Active bit = 1,
	@ShipType varchar(50),
	@XMLData xml
AS
	BEGIN TRANSACTION INSERTNEWFITTING
		INSERT INTO RecruitApplicationShipFittings(Name, [Description], Active, ShipType, XMLData)
		VALUES
		(@FittingName, @FittingDescription, @Active, @ShipType, @XMLData)
	COMMIT TRANSACTION INSERTNEWFITTING
		SELECT @@IDENTITY
RETURN 0

