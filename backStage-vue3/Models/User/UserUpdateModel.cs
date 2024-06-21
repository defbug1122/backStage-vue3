namespace backStage_vue3.Models
{
    /// <summary>
    /// 更新用戶資訊 POST 請求相關參數
    /// </summary>
    public class UserUpdateModel : UserSessionModel
    {
        /// <summary>
        /// 更新用戶資訊 POST-請求參數-欲更改的用戶名稱
        /// </summary>
        public string Un { get; set; }

        /// <summary>
        /// 更新用戶資訊 POST-請求參數-欲更改的用戶密碼
        /// </summary>
        public string Pwd { get; set; }

        /// <summary>
        /// 更新用戶資訊 POST-請求參數-欲更改的用戶權限
        /// </summary>
        public string Permission { get; set; }
    }
}