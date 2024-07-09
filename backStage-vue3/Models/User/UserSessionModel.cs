namespace backStage_vue3.Models
{
    /// <summary>
    /// 當前登入用戶 Session
    /// </summary>
    public class UserSessionModel
    {
        /// <summary>
        /// 當前登入用戶編號
        /// </summary>
       public int Id { get; set; }

        /// <summary>
        /// 當前登入用戶名稱
        /// </summary>
        public string UserName { get; set; }
        
        /// <summary>
        /// 當前登入用戶權限
        /// </summary>
        public int Permission { get; set; }

        /// <summary>
        /// 當前登入用戶sessionID
        /// </summary>
        public string SessionID { get; set; }
    }
}