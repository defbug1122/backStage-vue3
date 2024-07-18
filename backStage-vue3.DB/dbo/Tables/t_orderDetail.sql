CREATE TABLE [dbo].[t_orderDetail] (
    [f_orderDetailId] INT IDENTITY (1, 1) NOT NULL,
    [f_orderId]       INT NOT NULL,
    [f_productId]     INT NOT NULL,
    [f_quantity]      INT NOT NULL,
    PRIMARY KEY CLUSTERED ([f_orderDetailId] ASC),
    CONSTRAINT [FK_OrderDetail_Order] FOREIGN KEY ([f_orderId]) REFERENCES [dbo].[t_order] ([f_orderId]),
    CONSTRAINT [FK_OrderDetail_Product] FOREIGN KEY ([f_productId]) REFERENCES [dbo].[t_product] ([f_productId])
);



