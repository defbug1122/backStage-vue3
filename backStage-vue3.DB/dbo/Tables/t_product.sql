CREATE TABLE [dbo].[t_product] (
    [f_productId]  INT             IDENTITY (1, 1) NOT NULL,
    [f_name]       NVARCHAR (20)   NOT NULL,
    [f_imagePath1] VARCHAR (50)    NULL,
    [f_type]       TINYINT         NOT NULL,
    [f_price]      DECIMAL (10, 2) NOT NULL,
    [f_active]     BIT             CONSTRAINT [DF_t_product_f_active] DEFAULT ((0)) NOT NULL,
    [f_describe]   NVARCHAR (50)   NULL,
    [f_stock]      INT             CONSTRAINT [DF_t_product_f_stock] DEFAULT ((1)) NOT NULL,
    [f_imagePath2] VARCHAR (50)    NULL,
    [f_imagePath3] VARCHAR (50)    NULL,
    CONSTRAINT [PK_t_product] PRIMARY KEY CLUSTERED ([f_productId] ASC)
);



