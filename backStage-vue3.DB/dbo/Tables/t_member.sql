CREATE TABLE [dbo].[t_member] (
    [f_memberId]   INT             IDENTITY (1, 1) NOT NULL,
    [f_memberName] NVARCHAR (50)   NOT NULL,
    [f_level]      TINYINT         CONSTRAINT [DF_t_member_f_level] DEFAULT ((4)) NOT NULL,
    [f_status]     BIT             DEFAULT ((1)) NOT NULL,
    [f_totalSpent] DECIMAL (10, 2) CONSTRAINT [DF_t_member_f_totalSpent] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_t_member] PRIMARY KEY CLUSTERED ([f_memberId] ASC)
);










GO

CREATE TRIGGER [dbo].[trg_UpdateMemberLevel]
ON [dbo].[t_member]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    -- 更新 t_member 表中插入或更新的行的 f_level 字段
    UPDATE t_member
    SET f_level = 
        CASE
            WHEN INSERTED.f_totalSpent BETWEEN 0 AND 2000 THEN 1
            WHEN INSERTED.f_totalSpent BETWEEN 2001 AND 8000 THEN 2
            WHEN INSERTED.f_totalSpent BETWEEN 8001 AND 20000 THEN 3
            ELSE 4
        END
    FROM inserted
    WHERE t_member.f_memberId = inserted.f_memberId;
END;