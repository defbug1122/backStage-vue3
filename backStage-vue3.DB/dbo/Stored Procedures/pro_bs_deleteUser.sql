CREATE PROCEDURE [dbo].[pro_bs_deleteUser]
    @UserName NVARCHAR(50),
    @CurrentUserName NVARCHAR(50)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM t_account WHERE f_userName = @UserName)
    BEGIN
        IF @UserName = @CurrentUserName
        BEGIN
            SELECT 'Cannot delete yourself' AS message;
        END
        ELSE
        BEGIN
            DELETE FROM t_account WHERE f_userName = @UserName;
            SELECT 'User deleted successfully' AS message;
        END
    END
    ELSE
    BEGIN
        SELECT 'User not found' AS message;
    END
END