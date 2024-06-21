CREATE PROCEDURE [dbo].[pro_bs_getUuidByUn]
    @currentUn NVARCHAR(50)
AS
BEGIN
    SELECT f_uuid FROM t_acc WITH(NOLOCK) WHERE f_un = @currentUn;
END