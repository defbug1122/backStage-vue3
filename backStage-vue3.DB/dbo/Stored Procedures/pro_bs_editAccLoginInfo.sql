CREATE PROCEDURE [dbo].[pro_bs_editAccLoginInfo]
    @userId INT,
    @uuid NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE t_acc
    SET f_uuid = @uuid
    WHERE f_id = @userId;
END;