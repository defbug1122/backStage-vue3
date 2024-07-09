CREATE TABLE [dbo].[t_order] (
    [f_orderId]      INT           NOT NULL,
    [orderDate]      DATETIME2 (7) NOT NULL,
    [deliveryStatus] NVARCHAR (50) NOT NULL,
    [deliveryMethod] NVARCHAR (50) NOT NULL,
    [isReturn]       BIT           NOT NULL,
    CONSTRAINT [PK_t_order] PRIMARY KEY CLUSTERED ([f_orderId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_t_order]
    ON [dbo].[t_order]([f_orderId] ASC);

