namespace backStage_vue3.Models
{
    // <summary>
    /// 用戶列表資訊
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// 用戶列表-索引鍵
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 用戶列表-用戶名稱
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用戶列表-用戶創立時間
        /// </summary>
        public string CreateTime { get; set; }

        /// <summary>
        /// 用戶列表-用戶當前權限
        /// </summary>
        public int Permission { get; set; }
    }

    /// <summary>
    /// 用戶列表請求參數
    /// </summary>
    public class GetUserRequest
    {
        /// <summary>
        /// 用戶列表-關鍵字
        /// </summary>
        public string SearchTerm { get; set; } = "";

        /// <summary>
        /// 用戶列表-頁面第幾頁
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// 用戶列表-頁面顯示幾筆
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 用戶列表-排序類別
        /// </summary>
        public int SortBy { get; set; } = 1;
    }
}