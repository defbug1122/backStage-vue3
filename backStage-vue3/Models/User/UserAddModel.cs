using System;

namespace backStage_vue3.Models
{
    /// <summary>
    /// 新增用戶POST 請求相關參數
    /// </summary>
    public class UserAddModel
    {
        /// <summary>
        /// 新增用戶POST-請求參數-用戶編號
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 新增用戶POST-請求參數-用戶名稱
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 新增用戶POST-請求參數-用戶密碼
        /// </summary>
        public string Pwd { get; set; }

        /// <summary>
        /// 新增用戶POST-請求參數-用戶創立時間
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 新增用戶POST-請求參數-用戶權限
        /// </summary>
        public int? Permission { get; set; }
    }
}