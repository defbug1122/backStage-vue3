CREATE PROCEDURE [dbo].[pro_bs_getUuidByUserName]
    @UserName NVARCHAR(50)
AS
BEGIN
    SELECT f_uuid FROM t_account WHERE f_userName = @UserName;
END