CREATE PROCEDURE [dbo].[Web_Alt_Add_Alt]
	@mainName varchar(max),
	@altNAme varchar(max),
	@altPurpose varchar(max) 
AS
	IF NOT EXISTS
	(SELECT * FROM AltList WHERE AltName = @altNAme AND MainName = @mainName)
	BEGIN
		INSERT INTO AltList (MainName, AltName, AltPurpose)
		VALUES (@mainName, @altNAme, @altPurpose)
		SELECT 1
	END
	ELSE
	BEGIN
		SELECT 2
	END
SELECT 0
