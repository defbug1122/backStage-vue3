CREATE PROCEDURE [dbo].[pro_bs_delProduct]
    @productId INT,
    @statusCode INT OUTPUT,
	@imagePath1 VARCHAR(24) OUTPUT,
    @imagePath2 VARCHAR(24) OUTPUT,
    @imagePath3 VARCHAR(24) OUTPUT
AS
BEGIN
    -- 初始化圖片路徑
    SET @imagePath1 = NULL;
    SET @imagePath2 = NULL;
    SET @imagePath3 = NULL;

    -- 檢查商品是否存在
    IF EXISTS (SELECT 1 FROM t_product WITH(NOLOCK) WHERE f_productId = @productId)
    BEGIN
        -- 取得圖片路徑
        SELECT @imagePath1 = f_imagePath1, @imagePath2 = f_imagePath2, @imagePath3 = f_imagePath3
        FROM t_product
        WHERE f_productId = @productId;

        -- 刪除商品
        DELETE FROM t_product WHERE f_productId = @productId;
        SET @statusCode = 0;
    END
    ELSE
    BEGIN
        SET @statusCode = 1;
    END
END