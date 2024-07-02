CREATE PROCEDURE [dbo].[pro_bs_getUserLogout]
    @un VARCHAR(16),
    @statusCode INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- 清空 UUID
    UPDATE t_acc WITH(ROWLOCK)
    SET f_uuid = NULL
    WHERE f_un = @un;

    IF @@ROWCOUNT = 0
    BEGIN
        SET @statusCode = 8; -- 用戶不存在
    END
    ELSE
    BEGIN
        SET @statusCode = 0; -- 成功
    END
END;