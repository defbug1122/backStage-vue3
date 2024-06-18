CREATE PROCEDURE [dbo].[pro_bs_addNewUser]
    @UserName NVARCHAR(50),
    @Password NVARCHAR(50),
    @CreateTime DATETIME,
    @Permission NVARCHAR(50)
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM t_account WHERE f_userName = @UserName)
    BEGIN
        INSERT INTO t_account (f_userName, f_password, f_createTime, f_permission)
        VALUES (@UserName, @Password, @CreateTime, @Permission);
        SELECT 'User created successfully' AS message;
    END
    ELSE
    BEGIN
        SELECT 'Username already exists' AS message;
    END
END