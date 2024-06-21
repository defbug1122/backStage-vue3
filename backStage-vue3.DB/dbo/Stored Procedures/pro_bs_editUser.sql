CREATE PROCEDURE [dbo].[pro_bs_editUser]
    @currentUn NVARCHAR(50),
    @currentSessionID NVARCHAR(50),
    @un NVARCHAR(50),
    @newPwd NVARCHAR(64),
    @newPermission NVARCHAR(50),
    @updateTime DATETIME,
    @statusCode INT OUTPUT  -- 状态码输出参数
AS
BEGIN
    DECLARE @dbSessionID NVARCHAR(50);
    
    -- 獲取最新該用戶的 UUID
    SELECT @dbSessionID = f_uuid FROM t_acc WITH(NOLOCK) WHERE f_un = @currentUn;

    IF @dbSessionID IS NULL OR @dbSessionID != @currentSessionID
    BEGIN
        SET @statusCode = 5;
        RETURN;
    END

    -- 更新用戶
    IF EXISTS (SELECT 1 FROM t_acc WITH(NOLOCK) WHERE f_un = @un)
    BEGIN
        UPDATE t_acc WITH(ROWLOCK)
        SET f_pwd = @newPwd, f_permission = @newPermission, f_updateTime = @updateTime
        WHERE f_un = @un;
        SET @statusCode = 0;
    END
    ELSE
    BEGIN
        SET @statusCode = 1;
    END
END