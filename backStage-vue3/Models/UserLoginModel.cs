
namespace backStage_vue3.Models
{
    /// <summary> 用戶登入POST 請求相關參數
    public class UserLoginModel
    {
        /// <summary> 請求參數-用戶名稱
        public string Un { get; set; }

        /// <summary> 請求參數-用戶密碼
        ///
        /// </summary> 
        public string Pwd { get; set; }
       
        /// <summary> 請求參數-用戶辨識ID
        public string SessionId { get; set; }
    }
}