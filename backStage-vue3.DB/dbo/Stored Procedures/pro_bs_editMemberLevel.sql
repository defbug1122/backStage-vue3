
CREATE PROCEDURE [dbo].[pro_bs_editMemberLevel]
    @currentUn VARCHAR(16),
    @currentSessionID CHAR(24),
    @mId INT,
    @level TINYINT,
    @statusCode INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

	DECLARE @dbSessionID CHAR(24);

    SET @statusCode = 0;

    -- 獲取最新該用戶的 UUID
    SELECT @dbSessionID = f_uuid FROM t_acc WITH(NOLOCK) WHERE f_un = @currentUn;

    IF @dbSessionID IS NULL OR @dbSessionID != @currentSessionID
    BEGIN
        SET @statusCode = 5; 
        RETURN;
    END

	    -- 檢查會員是否存在
    IF NOT EXISTS (SELECT 1 FROM t_member WITH(NOLOCK) WHERE f_mId = @mId)
    BEGIN
        SET @statusCode = 7; -- 未找到會員
        RETURN;
    END

    -- 更新会员等级
    UPDATE t_member WITH(ROWLOCK)
    SET f_level = @level
    WHERE f_mId = @mId;

    SET @statusCode = 0; -- 更新成功
END