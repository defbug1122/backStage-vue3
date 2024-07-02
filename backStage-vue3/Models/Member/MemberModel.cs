namespace backStage_vue3.Models
{
    /// <summary>
    /// 會員列表資訊
    /// </summary>
    public class MemberModel 
    {
        /// <summary>
        /// 會員列表-索引鍵
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 會員列表-會員名稱
        /// </summary>
        public string Mn { get; set; }

        /// <summary>
        /// 會員列表-會員等級
        /// </summary>
        public byte Level { get; set; }

        /// <summary>
        /// 會員列表-消費金額
        /// </summary>
        public decimal TotalSpent { get; set; }

        /// <summary>
        /// 會員列表-會員帳號狀態
        /// </summary>
        public bool Status { get; set; }
    }
}