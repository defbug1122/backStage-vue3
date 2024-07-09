CREATE PROCEDURE [dbo].[pro_bs_addNewUser]
    @currentUserId INT,
    @currentSessionId CHAR(24),
    @userName VARCHAR(16),
    @pwd CHAR(64),
    @createTime DATETIME,
    @permission INT,
    @statusCode INT OUTPUT
AS
BEGIN
    -- 插入新用戶
    IF NOT EXISTS (SELECT 1 FROM t_acc WITH(NOLOCK) WHERE f_userName = @userName)
    BEGIN
        INSERT INTO t_acc (f_userName, f_pwd, f_createTime, f_permission)
        VALUES (@userName, @pwd, @createTime, @permission);
        SET @statusCode = 0;
    END
    ELSE
    BEGIN
        SET @statusCode = 1;
    END
END