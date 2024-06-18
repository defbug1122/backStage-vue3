CREATE PROCEDURE [dbo].[pro_bs_updateAccountLoginInfo]
    @UserId INT,
    @UUID NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE t_account
    SET f_uuid = @UUID
    WHERE f_id = @UserId;
END;
