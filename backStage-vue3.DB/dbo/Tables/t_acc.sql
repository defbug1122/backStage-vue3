CREATE TABLE [dbo].[t_acc] (
    [f_userId]     INT          IDENTITY (1, 1) NOT NULL,
    [f_userName]   VARCHAR (16) NULL,
    [f_permission] INT          NULL,
    [f_createTime] DATETIME     NOT NULL,
    [f_updateTime] DATETIME     NULL,
    [f_sessionId]  CHAR (24)    NULL,
    [f_pwd]        CHAR (64)    NULL,
    CONSTRAINT [PK_t_acc] PRIMARY KEY CLUSTERED ([f_userId] ASC)
);















