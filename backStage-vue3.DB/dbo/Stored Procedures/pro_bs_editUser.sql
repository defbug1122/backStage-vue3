CREATE PROCEDURE [dbo].[pro_bs_editUser]
    @un NVARCHAR(50),
    @newPassword NVARCHAR(50),
    @newPermission NVARCHAR(50),
    @currentUn NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    -- 檢查是否正在修改自己的帳號
    IF @un = @currentUn
    BEGIN
        SELECT 'Cannot edit your own account' AS Message;
        RETURN;
    END

    -- 更新用戶密碼和權限
    UPDATE t_acc
    SET f_pwd = @newPassword, f_permission = @newPermission
    WHERE f_un = @un;

    IF @@ROWCOUNT = 0
    BEGIN
        SELECT 'User not found' AS Message;
    END
    ELSE
    BEGIN
        SELECT 'User updated successfully' AS Message;
    END
END