CREATE PROCEDURE [dbo].[pro_bs_editDeliveryMethod]
    @orderId INT,
    @deliveryMethod TINYINT,
	@deliveryAddress NVARCHAR(50),
    @statusCode INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- 检查订单是否存在
    IF NOT EXISTS (SELECT 1 FROM t_order WHERE f_orderId = @orderId)
    BEGIN
        SET @statusCode = 1; -- 订单不存在
        RETURN;
    END

    -- 更新配送方式
    UPDATE t_order WITH(ROWLOCK)
        SET f_deliveryMethod = @deliveryMethod,
        f_deliveryAddress = @deliveryAddress
    WHERE f_orderId = @orderId;

    SET @statusCode = 0; -- 成功
END