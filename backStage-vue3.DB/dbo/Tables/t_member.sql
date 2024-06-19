CREATE TABLE [dbo].[t_member] (
    [f_mId]    INT           IDENTITY (1, 1) NOT NULL,
    [f_mn]     NVARCHAR (50) NOT NULL,
    [f_level]  NVARCHAR (50) NULL,
    [f_status] BIT           DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_t_member] PRIMARY KEY CLUSTERED ([f_mId] ASC)
);



