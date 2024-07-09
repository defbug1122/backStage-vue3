
CREATE PROCEDURE [dbo].[pro_bs_addNewProduct]
    @productName NVARCHAR(20),
    @imagePath VARCHAR(50),
    @productPrice DECIMAL(10, 2),
	@productType TINYINT,
	@productDescribe NVARCHAR(50),
	@productStock INT,
	@productActive BIT,
    @statusCode INT OUTPUT
AS
BEGIN
    -- 插入新用戶
    IF NOT EXISTS (SELECT 1 FROM t_product WITH(NOLOCK) WHERE f_name = @productName)
    BEGIN
        INSERT INTO t_product(f_name, f_imagePath, f_type, f_price, f_describe, f_stock, f_active)
        VALUES (@productName, @imagePath, @productType, @productPrice, @productDescribe, @productStock, @productActive);
        SET @statusCode = 0;
    END
    ELSE
    BEGIN
        SET @statusCode = 1;
    END
END