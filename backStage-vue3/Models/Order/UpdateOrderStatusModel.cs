namespace backStage_vue3.Models
{
    /// <summary>
    /// 更新訂單狀態 POST 請求參數
    /// </summary>
    public class UpdateOrderStatusModel
    {
        /// <summary>
        /// 訂單流水號
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 訂單狀態
        /// </summary>
        public byte OrderStatus { get; set; }
    }
}