﻿CREATE PROCEDURE [dbo].[pro_bs_getUserLogin]
    @un NVARCHAR(50),
    @pwd NVARCHAR(64),
    @uuid NVARCHAR(50), 
    @statusCode INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @userId INT;
    DECLARE @permission NVARCHAR(50);

    -- 查詢用戶ID以及權限
    SELECT @userId = f_id, @permission = f_permission
    FROM [t_acc] WITH(NOLOCK)
    WHERE f_un = @un AND f_pwd = @pwd;

    -- 如果找到該用戶，更新 UUID
    IF @userId IS NOT NULL
    BEGIN
        UPDATE t_acc WITH(ROWLOCK)
        SET f_uuid = @uuid
        WHERE f_id = @userId;

        SET @statusCode = 0;

        SELECT @userId AS f_id, @permission AS f_permission;
    END
    ELSE
    BEGIN
        SET @statusCode = 1;
    END
END;
