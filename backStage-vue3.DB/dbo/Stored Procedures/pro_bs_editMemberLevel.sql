
CREATE PROCEDURE [dbo].[pro_bs_editMemberLevel]
    @currentUserId INT,
    @currentSessionId CHAR(24),
	@currentPermission INT,
    @memberId INT,
    @level TINYINT,
    @statusCode INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

	-- 檢查會員是否存在
    IF NOT EXISTS (SELECT 1 FROM t_member WITH(NOLOCK) WHERE f_memberId = @memberId)
    BEGIN
        SET @statusCode = 7; -- 未找到會員
        RETURN;
    END

    -- 更新會員等级
    UPDATE t_member WITH(ROWLOCK)
    SET f_level = @level
    WHERE f_memberId = @memberId;

    SET @statusCode = 0; -- 更新成功
END