CREATE PROCEDURE pro_bs_editUser
    @UserName NVARCHAR(50),
    @NewPassword NVARCHAR(50),
    @NewPermission NVARCHAR(50),
    @CurrentUserName NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    -- 檢查是否正在修改自己的帳號
    IF @UserName = @CurrentUserName
    BEGIN
        SELECT 'Cannot edit your own account' AS Message;
        RETURN;
    END

    -- 更新用戶密碼和權限
    UPDATE t_member
    SET password = @NewPassword, permission = @NewPermission
    WHERE username = @UserName;

    IF @@ROWCOUNT = 0
    BEGIN
        SELECT 'User not found' AS Message;
    END
    ELSE
    BEGIN
        SELECT 'User updated successfully' AS Message;
    END
END