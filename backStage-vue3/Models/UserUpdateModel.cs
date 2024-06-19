namespace backStage_vue3.Models
{
    /// <summary> 更新用戶資訊 POST 請求相關參數
    public class UserUpdateModel
    {
        /// <summary> 請求參數-目前登入的用戶名稱
        public string CurrentUn { get; set; }

        /// <summary> 請求參數-欲更改的用戶名稱
        public string Un { get; set; }

        /// <summary> 請求參數-欲更改的用戶密碼
        public string Pwd { get; set; }

        /// <summary> 請求參數-欲更改的用戶權限
        public string Permission { get; set; }
    }
}