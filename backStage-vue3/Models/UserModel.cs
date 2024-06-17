using System;

namespace backStage_vue3.Models
{
    /// <summary> 用戶列表資訊
    public class UserModel
    {
        /// <summary> 索引鍵
        public int Id { get; set; }

        /// <summary> 用戶名
        public string UserName { get; set; }

        /// <summary> 用戶密碼
        public string Password { get; set; }

        /// <summary> 用戶最近登入時間
        public DateTime? LoginTime { get; set; }

        /// <summary> 用戶創立時間
        public DateTime? CreateTime { get; set; }

        /// <summary> 用戶當前權限
        public string Permission { get; set; }
    }
}