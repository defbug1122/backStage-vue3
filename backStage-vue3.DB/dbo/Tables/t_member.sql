CREATE TABLE [dbo].[t_member] (
    [f_memberId]   INT           IDENTITY (1, 1) NOT NULL,
    [f_memberName] NVARCHAR (50) NOT NULL,
    [f_level]      NVARCHAR (50) NULL,
    [f_status]     BIT           DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_t_member] PRIMARY KEY CLUSTERED ([f_memberId] ASC)
);

