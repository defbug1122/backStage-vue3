CREATE PROCEDURE [dbo].[pro_bs_editManualMemberLevels]
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE t_member
    SET f_level = CASE
        WHEN f_totalSpent BETWEEN 0 AND 2000 THEN 1
        WHEN f_totalSpent BETWEEN 2001 AND 8000 THEN 2
        WHEN f_totalSpent BETWEEN 8001 AND 20000 THEN 3
        WHEN f_totalSpent > 20000 THEN 4
        ELSE f_level
    END;
END;