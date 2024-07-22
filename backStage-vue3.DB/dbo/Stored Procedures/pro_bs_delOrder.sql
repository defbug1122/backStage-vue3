CREATE PROCEDURE [dbo].[pro_bs_delOrder]
    @orderId INT,
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

    -- 删除订单明细
    DELETE FROM t_orderDetail
    WHERE f_orderId = @orderId;

    -- 删除订单
    DELETE FROM t_order
    WHERE f_orderId = @orderId;

    SET @statusCode = 0; -- 成功
END