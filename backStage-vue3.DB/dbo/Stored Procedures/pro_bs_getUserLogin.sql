CREATE PROCEDURE [dbo].[pro_bs_getUserLogin]
    @userName VARCHAR(16),
    @pwd CHAR(64),
    @sessionId CHAR(24), 
    @statusCode INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @userId INT;
    DECLARE @permission INT;

    -- 查詢用戶ID以及權限
    SELECT @userId = f_userId, @permission = f_permission
    FROM [t_acc] WITH(NOLOCK)
    WHERE f_userName = @userName AND f_pwd = @pwd;

    -- 如果該用戶，更新 UUID
    IF @userId IS NOT NULL
    BEGIN
        UPDATE t_acc WITH(ROWLOCK)
        SET f_sessionId = @sessionId
        WHERE f_userId = @userId;

        SET @statusCode = 0;

        SELECT @userId AS f_userId, @permission AS f_permission;
    END
    ELSE
    BEGIN
        SET @statusCode = 1;
    END
END;
