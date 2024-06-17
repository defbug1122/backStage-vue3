using System;

namespace backStage_vue3.Models
{
    /// <summary> 新增用戶POST 請求相關參數
    public class UserAddModel
    {
        /// <summary> 請求參數-用戶名稱
        public string UserName { get; set; }

        /// <summary> 請求參數-用戶密碼
        public string Password { get; set; }

        /// <summary> 請求參數-用戶創立時間
        public DateTime? CreateTime { get; set; }

        /// <summary> 請求參數-用戶權限
        public string Permission { get; set; }
    }
}