namespace backStage_vue3.Models
{
    /// <summary>
    /// 當前登入用戶 Session
    /// </summary>
    public class UserSessionModel
    {
        /// <summary>
        /// 當前登入用戶名稱
        /// </summary>
        public string CurrentUser { get; set; }
        
        /// <summary>
        /// 當前登入用戶權限
        /// </summary>
        public string CurrentPermission { get; set; }
    }
}