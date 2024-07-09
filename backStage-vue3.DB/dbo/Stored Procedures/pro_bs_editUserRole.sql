CREATE PROCEDURE [dbo].[pro_bs_editUserRole]
    @userId INT,
    @newPermission INT,
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
            f_permission = CASE WHEN @newPermission IS NOT NULL THEN @newPermission ELSE f_permission END,
            f_updateTime = @updateTime
        WHERE f_userId = @userId;
        SET @statusCode = 0;
    END
    ELSE
    BEGIN
        SET @statusCode = 1;
    END
END