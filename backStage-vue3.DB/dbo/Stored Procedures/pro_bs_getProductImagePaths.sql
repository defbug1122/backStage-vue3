-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[pro_bs_getProductImagePaths]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT f_imagePath1, f_imagePath2, f_imagePath3
    FROM t_product
END