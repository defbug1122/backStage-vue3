using System;

namespace backStage_vue3.Models
{
    /// <summary>
    /// 用戶列表資訊
    /// </summary>
    public class UserModel : UserSessionModel
    {
        /// <summary>
        /// 用戶列表-索引鍵
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 用戶列表-用戶名
        /// </summary>
        public string Un { get; set; }

        /// <summary>
        /// 用戶列表-用戶密碼
        /// </summary>
        public string Pwd { get; set; }

        /// <summary>
        /// 用戶列表-用戶創立時間
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 用戶列表-用戶當前權限
        /// </summary>
        public string Permission { get; set; }
    }
}