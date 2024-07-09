CREATE PROCEDURE [dbo].[pro_bs_getAllMembers]
    @currentUserId INT,
    @currentSessionId CHAR(24),
	@currentPermission INT,
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
    SELECT f_memberId, f_memberName, f_level, f_status, f_totalSpent, COUNT(*) OVER() AS TotalRecords FROM t_member WITH(NOLOCK)

    WHERE 
        f_memberName LIKE '%' + @searchTerm + '%'
    ORDER BY 
        CASE 
            WHEN @sortBy = 1 THEN f_memberId
        END ASC,
        CASE
            WHEN @sortBy = 2 THEN f_level
        END ASC
    OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;

    SET @statusCode = 0;
END