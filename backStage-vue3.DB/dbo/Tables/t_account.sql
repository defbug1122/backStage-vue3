CREATE TABLE [dbo].[t_account] (
    [f_id]         INT           IDENTITY (1, 1) NOT NULL,
    [f_userName]   NVARCHAR (50) NOT NULL,
    [f_password]   NVARCHAR (50) NOT NULL,
    [f_permission] NVARCHAR (50) NULL,
    [f_createTime] DATETIME      NOT NULL,
    [f_updateTime] DATETIME      NULL,
    [f_loginTime]  DATETIME      NULL,
    [f_uuid]       NVARCHAR (50) NULL,
    CONSTRAINT [PK_t_account] PRIMARY KEY CLUSTERED ([f_id] ASC)
);

