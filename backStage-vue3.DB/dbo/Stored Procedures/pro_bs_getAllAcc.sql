﻿CREATE PROCEDURE [dbo].[pro_bs_getAllAcc]
    @currentUn VARCHAR(16),
    @currentSessionID CHAR(24),
    @searchTerm NVARCHAR(16) = "",
    @pageNumber INT,
    @pageSize INT,
	@sortBy INT = 1,
    @statusCode INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @dbSessionID CHAR(24);

    SET @statusCode = 0;

    -- 獲取最新該用戶的 UUID
    SELECT @dbSessionID = f_uuid FROM t_acc WITH(NOLOCK) WHERE f_un = @currentUn;

    IF @dbSessionID IS NULL OR @dbSessionID != @currentSessionID
    BEGIN
        SET @statusCode = 5; 
        RETURN;
    END

    -- 計算 OFFSET
    DECLARE @offset INT = (@pageNumber - 1) * @pageSize;

    -- 獲取用戶列表
    SELECT 
        f_id,
        f_un,
        f_pwd,
        f_createTime,
        f_Permission
    FROM 
        t_acc WITH(NOLOCK)
    WHERE 
        f_un LIKE '%' + @searchTerm + '%'
    ORDER BY 
		CASE 
            WHEN @sortBy = 1 THEN f_un
        END ASC,
        CASE
            WHEN @sortBy = 2 THEN f_createTime
        END DESC
    OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;

    -- 獲取總記錄數
    SELECT COUNT(*) AS TotalRecords
    FROM t_acc WITH(NOLOCK)
    WHERE f_un LIKE '%' + @searchTerm + '%';

    SET @statusCode = 0;
END