namespace backStage_vue3.Models
{
    public class UserEditInfoModel
    {
        /// <summary>
        /// 更新用戶密碼 POST-請求參數-欲更改的用戶編號
        /// </summary>
        public int Id { get; set; }


        /// <summary>
        /// 更新用戶密碼 POST-請求參數-欲更改的用戶密碼
        /// </summary>
        public string Pwd { get; set; }
    }
}