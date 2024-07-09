CREATE PROCEDURE [dbo].[pro_bs_getUserLogout]
    @currentUserId INT,
    @statusCode INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- 清空 UUID
    UPDATE t_acc WITH(ROWLOCK)
    SET f_sessionId = NULL
    WHERE f_userId = @currentUserId;

    IF @@ROWCOUNT = 0
    BEGIN
        SET @statusCode = 8; -- 用戶不存在
    END
    ELSE
    BEGIN
        SET @statusCode = 0; -- 成功
    END
END;