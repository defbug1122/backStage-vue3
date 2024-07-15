

CREATE PROCEDURE [dbo].[pro_bs_editProduct]
	@productId INT,
    @productName NVARCHAR(20),
    @imagePath1 VARCHAR(50),
	@imagePath2 VARCHAR(50),
	@imagePath3 VARCHAR(50),
    @productPrice DECIMAL(10, 2),
	@productType TINYINT,
	@productDescribe NVARCHAR(50),
	@productStock INT,
	@productActive BIT,
    @statusCode INT OUTPUT
AS
BEGIN
    -- 檢查商品是否存在
    IF NOT EXISTS (SELECT 1 FROM t_product WITH(NOLOCK) WHERE f_productId = @productId)
    BEGIN
        SET @statusCode = 14; -- 未找到該商品
        RETURN;
    END

	UPDATE t_product WITH(ROWLOCK)
	SET
		f_name = @productName,
		f_imagePath1 = CASE WHEN @imagePath1 = 'deleted' THEN NULL WHEN @imagePath1 != '' THEN @imagePath1 ELSE f_imagePath1 END,
		f_imagePath2 = CASE WHEN @imagePath2 = 'deleted' THEN NULL WHEN @imagePath2 != '' THEN @imagePath2 ELSE f_imagePath2 END,
		f_imagePath3 = CASE WHEN @imagePath3 = 'deleted' THEN NULL WHEN @imagePath3 != '' THEN @imagePath3 ELSE f_imagePath3 END,
		f_type = @productType,
		f_price = @productPrice,
		f_active = @productActive,
		f_describe = @productDescribe,
		f_stock = @productStock
    WHERE f_productId = @productId;


	SET @statusCode = 0; -- 更新成功
END