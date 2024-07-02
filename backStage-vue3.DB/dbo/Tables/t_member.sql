CREATE TABLE [dbo].[t_member] (
    [f_mId]        INT             IDENTITY (1, 1) NOT NULL,
    [f_mn]         NVARCHAR (50)   NOT NULL,
    [f_level]      TINYINT         CONSTRAINT [DF_t_member_f_level] DEFAULT ((4)) NOT NULL,
    [f_status]     BIT             DEFAULT ((1)) NOT NULL,
    [f_totalSpent] DECIMAL (10, 2) CONSTRAINT [DF_t_member_f_totalSpent] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_t_member] PRIMARY KEY CLUSTERED ([f_mId] ASC)
);





