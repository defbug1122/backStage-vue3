using System.Collections.Generic;

namespace backStage_vue3.Models
{
    /// <summary>
    /// 取得用戶列表 GET 回應
    /// </summary>
    public class GetUserResponseDto
    {
        /// <summary>
        /// 回應狀態碼
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 回應列表資料
        /// </summary>
        public List<UserModel> Data { get; set; }

        /// <summary>
        /// 回應是否還有下一頁
        /// </summary>
        public bool HasMore { get; set; }
    }
}