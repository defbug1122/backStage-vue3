CREATE PROCEDURE [dbo].[pro_bs_addNewUser]
    @un NVARCHAR(50),
    @pwd NVARCHAR(50),
    @createTime DATETIME,
    @permission NVARCHAR(50)
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM t_acc WHERE f_un = @un)
    BEGIN
        INSERT INTO t_acc (f_un, f_pwd, f_createTime, f_permission)
        VALUES (@un, @pwd, @createTime, @permission);
        SELECT 'User created successfully' AS message;
    END
    ELSE
    BEGIN
        SELECT 'Username already exists' AS message;
    END
END