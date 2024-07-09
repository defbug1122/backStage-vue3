CREATE PROCEDURE [dbo].[pro_bs_delUser]
    @userId INT,
    @statusCode INT OUTPUT
AS
BEGIN
    -- 刪除用戶
    IF EXISTS (SELECT 1 FROM t_acc WITH(NOLOCK) WHERE f_userId = @userId)
    BEGIN
        DELETE FROM t_acc WHERE f_userId = @userId;
        SET @statusCode = 0;
    END
    ELSE
    BEGIN
        SET @statusCode = 1;
    END
END