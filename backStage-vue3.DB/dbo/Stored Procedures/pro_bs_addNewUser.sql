CREATE PROCEDURE [dbo].[pro_bs_addNewUser]
    @currentUn NVARCHAR(50),
    @currentSessionID NVARCHAR(50),
    @un NVARCHAR(50),
    @pwd NVARCHAR(64),
    @createTime DATETIME,
    @permission NVARCHAR(50),
    @statusCode INT OUTPUT
AS
BEGIN
    DECLARE @dbSessionID NVARCHAR(50);
    
    -- 獲取最新該用戶的 UUID
    SELECT @dbSessionID = f_uuid FROM t_acc WITH(NOLOCK) WHERE f_un = @currentUn;

    -- 验证 sessionID
    IF @dbSessionID IS NULL OR @dbSessionID != @currentSessionID
    BEGIN
        SET @statusCode = 5;
        RETURN;
    END

    -- 插入新用戶
    IF NOT EXISTS (SELECT 1 FROM t_acc WITH(NOLOCK) WHERE f_un = @un)
    BEGIN
        INSERT INTO t_acc (f_un, f_pwd, f_createTime, f_permission)
        VALUES (@un, @pwd, @createTime, @permission);
        SET @statusCode = 0;
    END
    ELSE
    BEGIN
        SET @statusCode = 1;
    END
END