CREATE PROCEDURE [dbo].[pro_bs_getUserLogin]
    @UserName nvarchar(50),
	@Password nvarchar(50)
AS
BEGIN
    SELECT f_id, f_permission, f_loginTime
    FROM [t_account]
    WHERE f_userName = @UserName AND f_password = @Password
END
