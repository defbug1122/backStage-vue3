namespace backStage_vue3.Models
{
    /// <summary>
    /// 刪除訂單 POST 請求參數
    /// </summary>
    public class DeleteOrderModel
    {
        /// <summary>
        /// 訂單流水號
        /// </summary>
        public int OrderId { get; set; }
    }
}