CREATE PROCEDURE [dbo].[pro_bs_getAllAcc]
    @searchTerm NVARCHAR(16) = "",
    @pageNumber INT,
    @pageSize INT,
	@sortBy INT = 1,
    @statusCode INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- 計算 OFFSET
    DECLARE @offset INT = (@pageNumber - 1) * @pageSize;

    -- 獲取用戶列表和總記錄數
    SELECT f_userId, f_userName, f_pwd, f_createTime, f_Permission, COUNT(*) OVER() AS TotalRecords FROM t_acc WITH(NOLOCK)
    WHERE 
        f_userName LIKE '%' + @searchTerm + '%'
    ORDER BY 
        CASE 
            WHEN @sortBy = 1 THEN f_userName
        END ASC,
        CASE
            WHEN @sortBy = 2 THEN f_createTime
        END DESC
    OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;

    SET @statusCode = 0;
END