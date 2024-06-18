CREATE PROCEDURE [pro_bs_getAllAccounts]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM t_account;
END;