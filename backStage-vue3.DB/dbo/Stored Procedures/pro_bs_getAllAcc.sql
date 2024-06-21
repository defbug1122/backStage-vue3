CREATE PROCEDURE [dbo].[pro_bs_getAllAcc]
    @searchTerm NVARCHAR(50) = '',
    @pageNumber INT,
    @pageSize INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @startRow INT = (@pageNumber - 1) * @pageSize + 1;
    DECLARE @endRow INT = @startRow + @pageSize - 1;

    ;WITH UserCTE AS
    (
        SELECT 
            ROW_NUMBER() OVER (ORDER BY f_createTime DESC) AS RowNum,
            f_id,
            f_un,
            f_pwd,
            f_createTime,
            f_Permission
        FROM 
            t_acc WITH (NOLOCK)
        WHERE 
            f_un LIKE '%' + @searchTerm + '%'
    )
    SELECT 
        f_id,
        f_un,
        f_pwd,
        f_createTime,
        f_Permission
    FROM 
        UserCTE
    WHERE 
        RowNum BETWEEN @startRow AND @endRow;

    SELECT COUNT(*) AS TotalRecords
    FROM t_acc WITH (NOLOCK)
    WHERE f_un LIKE '%' + @searchTerm + '%';
END