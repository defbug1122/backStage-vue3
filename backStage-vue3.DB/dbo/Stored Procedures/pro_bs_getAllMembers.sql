CREATE PROCEDURE [dbo].[pro_bs_getAllMembers]
    @currentUn VARCHAR(16),
    @currentSessionID CHAR(24),
    @searchTerm VARCHAR(16) = "",
    @pageNumber INT,
    @pageSize INT,
    @sortBy INT = 1,
    @statusCode INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @dbSessionID CHAR(24);
    DECLARE @offset INT = (@pageNumber - 1) * @pageSize;

    SET @statusCode = 0;

    -- 獲取最新該用戶的 UUID
    SELECT @dbSessionID = f_uuid FROM t_acc WITH(NOLOCK) WHERE f_un = @currentUn;

    IF @dbSessionID IS NULL OR @dbSessionID != @currentSessionID
    BEGIN
        SET @statusCode = 5; 
        RETURN;
    END

    -- 獲取用戶列表
    SELECT 
        f_mId,
        f_mn,
        f_level,
        f_status,
		f_totalSpent
    FROM 
        t_member WITH(NOLOCK)
    WHERE 
        f_mn LIKE '%' + @searchTerm + '%'
    ORDER BY 
        CASE 
            WHEN @sortBy = 1 THEN f_mId
        END ASC,
        CASE
            WHEN @sortBy = 2 THEN f_level
        END ASC
    OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;

    SELECT COUNT(*) AS TotalRecords
    FROM t_member WITH(NOLOCK)
    WHERE f_mn LIKE '%' + @searchTerm + '%';

    SET @statusCode = 0;
END