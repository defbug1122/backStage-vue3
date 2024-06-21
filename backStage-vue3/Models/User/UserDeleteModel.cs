﻿namespace backStage_vue3.Models
{
    /// <summary>
    /// 刪除用戶POST 請求相關參數
    /// </summary>
    public class UserDeleteModel : UserSessionModel
    {
        /// <summary>
        /// 刪除用戶POST-請求參數-用戶名稱
        /// </summary>
        public string Un { get; set; }
    }
}