using System.Collections.Generic;

namespace backStage_vue3.Models
{
    /// <summary>
    /// 訂單資訊回應
    /// </summary>
    public class GetOrderResponseDto
    {
        /// <summary>
        /// 回應狀態碼
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 回應列表資料
        /// </summary>
        public List<OrderModel> Data { get; set; }

        /// <summary>
        /// 是否還有下一頁
        /// </summary>
        public bool HasMore { get; set; }
    }
}