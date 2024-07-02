CREATE PROCEDURE [dbo].[pro_bs_editUser]
    @currentUn VARCHAR(16),
    @currentSessionID CHAR(24),
    @un VARCHAR(16),
    @newPwd CHAR(64),
    @newPermission INT,
    @updateTime DATETIME,
    @statusCode INT OUTPUT
AS
BEGIN
    DECLARE @dbSessionID CHAR(24);
    
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
        SET 
            f_pwd = CASE WHEN @newPwd IS NOT NULL THEN @newPwd ELSE f_pwd END,
            f_permission = CASE WHEN @newPermission IS NOT NULL THEN @newPermission ELSE f_permission END,
            f_updateTime = @updateTime
        WHERE f_un = @un;
        SET @statusCode = 0;
    END
    ELSE
    BEGIN
        SET @statusCode = 1;
    END
END