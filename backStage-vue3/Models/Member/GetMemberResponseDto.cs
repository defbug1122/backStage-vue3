using System.Collections.Generic;

namespace backStage_vue3.Models
{
    /// <summary>
    /// 取得會員列表 GET 回應
    /// </summary>
    public class GetMemberResponseDto
    {
        /// <summary>
        /// 回應狀態碼
        /// </summary>
        public int Code { get; set; }

        public List<MemberModel> Data { get; set; }

        /// <summary>
        /// 回應是否還有下一頁
        /// </summary>
        public bool HasMore { get; set; }
    }
}