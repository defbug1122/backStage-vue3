CREATE PROCEDURE [dbo].[pro_bs_getAllAcc]
    @currentUn NVARCHAR(50),
    @currentSessionID NVARCHAR(50),
    @searchTerm NVARCHAR(50) = '',
    @pageNumber INT,
    @pageSize INT,
    @statusCode INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON; -- 禁止返回每個操作的影響行数的消息，提高性能

    DECLARE @dbSessionID NVARCHAR(50);
    DECLARE @startRow INT = (@pageNumber - 1) * @pageSize + 1;
    DECLARE @endRow INT = @startRow + @pageSize - 1;

    SET @statusCode = 0;

    -- 獲取最新該用戶的 UUID
    SELECT @dbSessionID = f_uuid FROM t_acc WITH(NOLOCK) WHERE f_un = @currentUn;

    IF @dbSessionID IS NULL OR @dbSessionID != @currentSessionID
    BEGIN
        SET @statusCode = 5; 
        RETURN;
    END

    -- 獲取用戶列表，先使用臨時結果集
    ;WITH UserCTE AS
    (
        SELECT 
            ROW_NUMBER() OVER (ORDER BY f_createTime DESC) AS RowNum,
            f_id,
            f_un,
            f_pwd,
            f_createTime,
            f_Permission
        FROM 
            t_acc WITH(NOLOCK)
        WHERE 
            f_un LIKE '%' + @searchTerm + '%'
    )
    SELECT 
        f_id,
        f_un,
        f_pwd,
        f_createTime,
        f_Permission
    FROM 
        UserCTE WITH(NOLOCK)
    WHERE 
        RowNum BETWEEN @startRow AND @endRow;

    SELECT COUNT(*) AS TotalRecords
    FROM t_acc WITH(NOLOCK)
    WHERE f_un LIKE '%' + @searchTerm + '%';
    SET @statusCode = 0;
END