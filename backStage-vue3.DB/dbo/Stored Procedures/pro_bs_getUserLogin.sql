CREATE PROCEDURE [dbo].[pro_bs_getUserLogin]
    @un nvarchar(50),
	@pwd nvarchar(50)
AS
BEGIN
    SELECT f_id, f_permission
    FROM [t_acc]
    WHERE f_un = @un AND f_pwd = @pwd
END
