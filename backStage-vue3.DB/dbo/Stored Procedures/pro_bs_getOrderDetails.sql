CREATE PROCEDURE [dbo].[pro_bs_getOrderDetails]
    @orderId INT,
    @statusCode INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SET @statusCode = 0;

    -- 獲取訂單訊息
    SELECT o.f_orderId AS OrderId, o.f_orderNumber AS OrderNumber, o.f_orderDate AS OrderDate, m.f_memberName AS MemberName, o.f_receiver AS Receiver, o.f_mobileNumber AS MobileNumber, o.f_deliveryAddress AS DeliveryAddress, o.f_deliveryMethod AS DeliveryMethod, o.f_deliveryStatus AS DeliveryStatus, o.f_orderAmount AS OrderAmount, o.f_orderStatus AS OrderStatus, o.f_returnStatus AS ReturnStatus
    FROM t_order o WITH(NOLOCK)
    JOIN t_member m WITH(NOLOCK) ON o.f_memberId = m.f_memberId
    WHERE o.f_orderId = @orderId;

    -- 獲取訂單明細訊息
    SELECT od.f_orderDetailId AS OrderDetailId, od.f_productId AS ProductId, p.f_name AS ProductName, od.f_quantity AS Quantity, p.f_price AS Price, (od.f_quantity * p.f_price) AS Subtotal
    FROM t_orderDetail od WITH(NOLOCK)
    JOIN t_product p WITH(NOLOCK) ON od.f_productId = p.f_productId
    WHERE od.f_orderId = @orderId;

    SET @statusCode = 0;
END