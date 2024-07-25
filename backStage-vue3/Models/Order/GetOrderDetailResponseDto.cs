using backStage_vue3.Models.Order;

namespace backStage_vue3.Models
{
    /// <summary>
    /// 訂單明細請求回應
    /// </summary>
    public class GetOrderDetailResponseDto
    {
        /// <summary>
        /// 回應狀態碼
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 回應列表資料
        /// </summary>
        public OrderDetailResponse Data { get; set; }
    }
}