-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Web_Contract_Create_New_Contract] 
	@Creator varchar(255),
	@Total decimal(18, 2)
AS
BEGIN
	BEGIN TRANSACTION InsertVals
	
	INSERT INTO [dbo].[Contracts](Creator, Created, Total, [Status])
	VALUES(@Creator, GetDate(), @Total, 'OUTSTANDING')

	COMMIT TRANSACTION InsertVals
	SELECT @@IDENTITY
END

