CREATE TABLE [dbo].[t_acc] (
    [f_id]         INT           IDENTITY (1, 1) NOT NULL,
    [f_un]         NVARCHAR (50) NOT NULL,
    [f_pwd]        NVARCHAR (50) NOT NULL,
    [f_permission] NVARCHAR (50) NULL,
    [f_createTime] DATETIME      NOT NULL,
    [f_updateTime] DATETIME      NULL,
    [f_uuid]       NVARCHAR (50) NULL,
    CONSTRAINT [PK_t_acc] PRIMARY KEY CLUSTERED ([f_id] ASC)
);









