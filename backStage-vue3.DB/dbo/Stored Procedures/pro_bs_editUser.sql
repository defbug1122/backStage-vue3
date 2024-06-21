CREATE PROCEDURE [dbo].[pro_bs_editUser]
    @un NVARCHAR(50),
    @newPwd NVARCHAR(64),
    @newPermission NVARCHAR(50),
    @currentUn NVARCHAR(50),
	@updateTime DATETIME,
	@statusCode INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON

    UPDATE t_acc
    SET f_pwd = @newPwd, f_permission = @newPermission, f_updateTime = @updateTime
    WHERE f_un = @un;

    IF @@ROWCOUNT = 0
    BEGIN
        SET @statusCode = 1;
    END
    ELSE
    BEGIN
        SET @statusCode = 0;
    END
END