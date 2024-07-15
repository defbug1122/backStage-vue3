
CREATE PROCEDURE [dbo].[pro_bs_delProduct]
    @productId INT,
    @statusCode INT OUTPUT
AS
BEGIN
    -- 刪除商品
    IF EXISTS (SELECT 1 FROM t_product WITH(NOLOCK) WHERE f_productId = @productId)
    BEGIN
        DELETE FROM t_product WHERE f_productId = @productId;
        SET @statusCode = 0;
    END
    ELSE
    BEGIN
        SET @statusCode = 1;
    END
END