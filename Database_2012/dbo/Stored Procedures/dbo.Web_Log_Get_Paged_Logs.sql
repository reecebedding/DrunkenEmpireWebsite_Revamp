
CREATE PROCEDURE [dbo].[Web_Log_Get_Paged_Logs]
	@pageNumber AS INT,
	@pageSize AS INT = 100
AS
	SELECT * FROM Logs
	ORDER BY TimeStamp DESC
	OFFSET ((@pageNumber - 1) * @pageSize) ROWS
	FETCH NEXT @pageSize ROWS ONLY


RETURN 0

