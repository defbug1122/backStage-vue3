
CREATE PROCEDURE [dbo].[pro_bs_getAllOrders]
    @searchTerm NVARCHAR(17) = "",
    @pageNumber INT,
    @pageSize INT,
    @sortBy INT = 1,
    @statusCode INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @offset INT = (@pageNumber - 1) * @pageSize;

    -- 獲取訂單列表
    SELECT o.f_orderId AS OrderId, o.f_orderDate AS OrderDate, o.f_orderNumber AS OrderNumber, m.f_memberName AS MemberName, o.f_receiver AS Receiver, o.f_orderAmount AS OrderAmount, o.f_orderStatus AS OrderStatus, COUNT(*) OVER() AS TotalRecords
    FROM 
        t_order o WITH(NOLOCK)
    JOIN 
        t_member m WITH(NOLOCK) ON o.f_memberId = m.f_memberId
    WHERE 
        o.f_orderNumber LIKE '%' + @searchTerm + '%'
    ORDER BY 
        CASE 
            WHEN @sortBy = 1 THEN o.f_orderId
            WHEN @sortBy = 2 THEN o.f_orderDate
            WHEN @sortBy = 3 THEN o.f_deliveryStatus
        END ASC
    OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;

    SET @statusCode = 0;
END