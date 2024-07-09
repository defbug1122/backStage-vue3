namespace backStage_vue3.Models
{
    /// <summary>
    /// 更新用戶權限 POST 請求相關參數
    /// </summary>
    public class UserEditRoleModel
    {
        /// <summary>
        /// 更新用戶權限 POST-請求參數-欲更改的用戶編號
        /// </summary>
        public int Id { get; set; }


        /// <summary>
        /// 更新用戶權限 POST-請求參數-欲更改的用戶權限
        /// </summary>
        public int Permission { get; set; }
    }
}