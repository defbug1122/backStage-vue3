CREATE PROCEDURE [dbo].[pro_bs_editUserInfo]
    @currentUserId INT,
    @currentSessionId CHAR(24),
	@currentPermission INT,
    @userId INT,
    @newPwd CHAR(64),
    @updateTime DATETIME,
    @statusCode INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
    -- 更新用戶
    IF EXISTS (SELECT 1 FROM t_acc WITH(NOLOCK) WHERE f_userId = @userId)
    BEGIN
        UPDATE t_acc WITH(ROWLOCK)
        SET 
            f_pwd = CASE WHEN @newPwd IS NOT NULL THEN @newPwd ELSE f_pwd END,
            f_updateTime = @updateTime
        WHERE f_userId = @userId;
        SET @statusCode = 0;
    END
    ELSE
    BEGIN
        SET @statusCode = 1;
    END
END