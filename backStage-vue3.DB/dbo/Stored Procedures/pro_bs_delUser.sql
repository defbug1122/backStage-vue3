CREATE PROCEDURE [dbo].[pro_bs_delUser]
    @currentUn VARCHAR(16),
    @currentSessionID CHAR(24),
    @un VARCHAR(16),
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

    -- 刪除用戶
    IF EXISTS (SELECT 1 FROM t_acc WITH(NOLOCK) WHERE f_un = @un)
    BEGIN
        DELETE FROM t_acc WHERE f_un = @un;
        SET @statusCode = 0;
    END
    ELSE
    BEGIN
        SET @statusCode = 1;
    END
END