namespace backStage_vue3.Models
{
    /// <summary>
    /// 更新配送資訊 POST 請求參數
    /// </summary>
    public class UpdateDeliveryMethodModel
    {
        /// <summary>
        /// 訂單流水號
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 訂單配送方式
        /// </summary>
        public byte DeliveryMethod { get; set; }

        /// <summary>
        /// 訂單配送地址
        /// </summary>
        public string DeliveryAddress { get; set; }

        /// <summary>
        /// 訂單收貨人
        /// </summary>
        public string Receiver { get; set; }

        /// <summary>
        /// 訂單聯絡手機號碼
        /// </summary>
        public string MobileNumber { get; set; }
    }
}