namespace backStage_vue3.Models
{
    /// <summary>
    /// 會員更新等級 POST 請求
    /// </summary>
    public class UpdateLevelModel
    {
        /// <summary>
        /// 會員編號
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        /// 會員等級
        /// </summary>
        public byte Level { get; set; }
    }
}