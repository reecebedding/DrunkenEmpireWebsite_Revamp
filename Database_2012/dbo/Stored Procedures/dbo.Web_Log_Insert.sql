--Description: Inserts a log record into the log table

CREATE PROCEDURE [dbo].[Web_Log_Insert]
	@TimeStamp VARCHAR(MAX) = null,
	@Level VARCHAR(MAX) = null,
	@IPAddress VARCHAR(MAX) = null,
	@User VARCHAR(MAX) = null,
	@CallSiteClass VARCHAR(MAX) = null,
	@StackTrace VARCHAR(MAX) = null,
	@Message VARCHAR(MAX) = null
AS
BEGIN
	INSERT INTO Logs ([TimeStamp], [Level], [IPAddress], [User], CallSiteClass, StackTrace, [Message])
	VALUES (@TimeStamp, @Level, @IPAddress, @User, @CallSiteClass, @StackTrace, @Message)
END
