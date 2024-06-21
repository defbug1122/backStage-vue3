CREATE PROCEDURE [dbo].[pro_bs_addNewUser]
    @un NVARCHAR(50),
    @pwd NVARCHAR(64),
    @createTime DATETIME,
    @permission NVARCHAR(50),
    @statusCode INT OUTPUT  -- 新增状态码输出参数
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM t_acc WITH(NOLOCK) WHERE f_un = @un)
    BEGIN
        INSERT INTO t_acc (f_un, f_pwd, f_createTime, f_permission)
        VALUES (@un, @pwd, @createTime, @permission);
        SET @statusCode = 0;
    END
    ELSE
    BEGIN
        SET @statusCode = 1;
    END
END