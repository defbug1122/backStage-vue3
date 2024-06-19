CREATE TABLE [dbo].[t_orders] (
    [orderNumber]    INT           NOT NULL,
    [orderDate]      DATETIME2 (7) NOT NULL,
    [deliveryStatus] NVARCHAR (50) NOT NULL,
    [deliveryMethod] NVARCHAR (50) NOT NULL,
    [isReturn]       BIT           NOT NULL
);

