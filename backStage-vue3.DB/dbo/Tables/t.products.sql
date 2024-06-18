CREATE TABLE [dbo].[t.products] (
    [id]          INT           NOT NULL,
    [name]        NVARCHAR (50) NOT NULL,
    [ImageUrl]    NVARCHAR (50) NOT NULL,
    [type]        NVARCHAR (50) NOT NULL,
    [price]       INT           NOT NULL,
    [isOpen]      BIT           NOT NULL,
    [description] NVARCHAR (50) NULL,
    [stock]       INT           NOT NULL
);

