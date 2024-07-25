namespace backStage_vue3.Models
{
    /// <summary>
    /// 更新配送進度 POST 請求參數
    /// </summary>
    public class UpdateDeliveryStatusModel
    {
        /// <summary>
        /// 訂單流水號
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 訂單配送狀態
        /// </summary>
        public byte DeliveryStatus { get; set; }
    }
}