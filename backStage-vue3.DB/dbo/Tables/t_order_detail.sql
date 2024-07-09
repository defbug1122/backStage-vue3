CREATE TABLE [dbo].[t_order_detail] (
    [f_orderProductId] INT IDENTITY (1, 1) NOT NULL,
    [f_orderId]        INT NOT NULL,
    [f_productId]      INT NOT NULL,
    [f_quantity]       INT NOT NULL,
    PRIMARY KEY CLUSTERED ([f_orderProductId] ASC),
    FOREIGN KEY ([f_orderId]) REFERENCES [dbo].[t_order] ([f_orderId]),
    CONSTRAINT [FK__t_order_p__f_pro__4E53A1AA] FOREIGN KEY ([f_productId]) REFERENCES [dbo].[t_product] ([f_productId])
);

