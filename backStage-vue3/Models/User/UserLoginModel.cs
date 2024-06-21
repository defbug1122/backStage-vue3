
using System.ComponentModel.DataAnnotations;

namespace backStage_vue3.Models
{
    /// <summary>
    /// 用戶登入POST 請求相關參數
    /// </summary>
    public class UserLoginModel
    {
        /// <summary>
        /// 用戶登入POST-請求參數-用戶名稱
        /// </summary>
        [Required]
        public string Un { get; set; }

        /// <summary>
        /// 用戶登入POST-請求參數-用戶密碼
        /// </summary> 
        [Required]
        public string Pwd { get; set; }

        /// <summary>
        /// 用戶登入POST-請求參數-用戶辨識ID
        /// </summary>
        public string SessionId { get; set; }
    }
}