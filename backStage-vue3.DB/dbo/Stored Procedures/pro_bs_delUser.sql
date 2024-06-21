CREATE PROCEDURE [dbo].[pro_bs_delUser]
    @un NVARCHAR(50),
    @currentUn NVARCHAR(50),
	@statusCode INT OUTPUT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM t_acc WITH(NOLOCK) WHERE f_un = @un)
    BEGIN
    BEGIN
        DELETE FROM t_acc WHERE f_un = @un;
		SET @statusCode = 0;
    END
    END
    ELSE
    BEGIN
         SET @statusCode = 1;
    END
END