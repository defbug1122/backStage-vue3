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
        public int MemberId { get; set; }

        /// <summary>
        /// 會員列表-會員名稱
        /// </summary>
        public string MemberName { get; set; }

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

        /// <summary>
        /// 會員列表-關鍵字
        /// </summary>
        public string SearchTerm { get; set; } = "";

        /// <summary>
        /// 會員列表-頁面第幾頁
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// 會員列表-頁面顯示幾筆
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 會員列表-排序類別
        /// </summary>
        public int SortBy { get; set; } = 1;
    }
}