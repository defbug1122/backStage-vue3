namespace backStage_vue3.Models
{
    /// <summary>
    /// 訂單狀態
    /// </summary>
    public enum OrderStatusModel
    {
        /// <summary>
        /// 處理中
        /// </summary>
        Processing = 1,

        /// <summary>
        /// 已確認
        /// </summary>
        Confirmed = 2,

        /// <summary>
        /// 已取消
        /// </summary>
        Canceled = 3,    
        
        /// <summary>
        /// 訂單已完成
        /// </summary>
        Completed = 4,

        /// <summary>
        /// 宅配-配送失敗
        /// </summary>
        DeliveryFailed = 5,

        /// <summary>
        /// 自取-未取貨
        /// </summary>
        SelfPickupNotCollected = 6,

        /// <summary>
        /// 退貨處理中
        /// </summary>
        ReturnProcessing = 7,

        /// <summary>
        /// 退貨完成
        /// </summary>
        ReturnCompleted = 8     
    }
}