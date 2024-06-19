CREATE PROCEDURE [dbo].[pro_bs_delUser]
    @un NVARCHAR(50),
    @currentUn NVARCHAR(50)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM t_acc WHERE f_un = @un)
    BEGIN
        IF @un = @currentUn
        BEGIN
            SELECT 'Cannot delete yourself' AS message;
        END
        ELSE
        BEGIN
            DELETE FROM t_acc WHERE f_un = @un;
            SELECT 'User deleted successfully' AS message;
        END
    END
    ELSE
    BEGIN
        SELECT 'User not found' AS message;
    END
END