CREATE TABLE [dbo].[t_order] (
    [f_orderId]         INT             IDENTITY (1, 1) NOT NULL,
    [f_orderDate]       DATETIME        NOT NULL,
    [f_memberId]        INT             NOT NULL,
    [f_receiver]        NVARCHAR (15)   NOT NULL,
    [f_mobileNumber]    CHAR (10)       NOT NULL,
    [f_deliveryMethod]  TINYINT         NOT NULL,
    [f_deliveryCounty]  NCHAR (3)       NULL,
    [f_deliveryCity]    NVARCHAR (3)    NULL,
    [f_deliveryAddress] NVARCHAR (50)   NULL,
    [f_deliveryStatus]  TINYINT         NOT NULL,
    [f_orderStatus]     TINYINT         CONSTRAINT [DF_t_order_f_orderStatus] DEFAULT ((1)) NOT NULL,
    [f_returnStatus]    TINYINT         CONSTRAINT [DF_t_order_f_returnStatus] DEFAULT ((1)) NOT NULL,
    [f_orderAmount]     DECIMAL (10, 2) NOT NULL,
    CONSTRAINT [PK_t_order] PRIMARY KEY CLUSTERED ([f_orderId] ASC),
    CONSTRAINT [FK_Order_Member] FOREIGN KEY ([f_memberId]) REFERENCES [dbo].[t_member] ([f_memberId])
);




GO
CREATE NONCLUSTERED INDEX [IX_t_order]
    ON [dbo].[t_order]([f_orderId] ASC);

