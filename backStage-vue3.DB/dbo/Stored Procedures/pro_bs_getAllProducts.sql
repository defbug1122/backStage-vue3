
CREATE PROCEDURE [dbo].[pro_bs_getAllProducts]
    @searchTerm VARCHAR(16) = "",
    @pageNumber INT,
    @pageSize INT,
    @sortBy INT = 1,
    @statusCode INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @offset INT = (@pageNumber - 1) * @pageSize;

    -- 獲取用戶列表
    SELECT f_productId, f_name, f_imagePath1, f_imagePath2, f_imagePath3, f_type, f_price, f_active, f_describe, f_stock, COUNT(*) OVER() AS TotalRecords FROM t_product WITH(NOLOCK)

    WHERE 
        f_name LIKE '%' + @searchTerm + '%'
    ORDER BY 
        CASE 
            WHEN @sortBy = 1 THEN f_name
        END ASC,
        CASE
            WHEN @sortBy = 2 THEN f_type
        END ASC
    OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;

    SET @statusCode = 0;
END