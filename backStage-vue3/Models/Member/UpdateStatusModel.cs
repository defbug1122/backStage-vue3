namespace backStage_vue3.Models
{
    /// <summary>
    /// 會員更新狀態 POST 資訊
    /// </summary>
    public class UpdateStatusModel
    {
        /// <summary>
        /// 會員編號
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        /// 會員狀態
        /// </summary>
        public bool Status { get; set; }
    }
}