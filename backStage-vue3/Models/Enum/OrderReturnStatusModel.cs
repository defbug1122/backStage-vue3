namespace backStage_vue3.Models
{
    /// <summary>
    /// 訂單退貨狀態
    /// </summary>
    public enum OrderReturnStatusModel
    {
        /// <summary>
        /// 未申請退貨
        /// </summary>
        NotApplied = 1,

        /// <summary>
        /// 退貨申請中
        /// </summary>
        Applying = 2,

        /// <summary>
        /// 退貨成功
        /// </summary>
        Successful = 3    
    }
}