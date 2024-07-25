namespace backStage_vue3.Models
{
    /// <summary>
    /// 訂單資訊
    /// </summary>
    public class OrderModel
    {
        /// <summary>
        /// 訂單資訊-訂單流水號
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 訂單資訊-訂單號碼
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// 訂單資訊-訂單日期
        /// </summary>
        public string OrderDate { get; set; }

        /// <summary>
        /// 訂單資訊-會員名稱
        /// </summary>
        public string MemberName { get; set; }

        /// <summary>
        /// 訂單資訊-收貨人
        /// </summary>
        public string Receiver { get; set; }

        /// <summary>
        /// 訂單資訊-訂單金額
        /// </summary>
        public decimal OrderAmount { get; set; }

        /// <summary>
        /// 訂單資訊-訂單狀態
        /// </summary>
        public byte OrderStatus { get; set; }

        /// <summary>
        /// 訂單資訊-配送狀態
        /// </summary>
        public byte DeliveryStatus { get; set; }
    }

    /// <summary>
    /// 訂單資訊 請求參數
    /// </summary>
    public class GetOrderRequest
    {
        /// <summary>
        /// 請求參數-關鍵字
        /// </summary>
        public string SearchTerm { get; set; } = "";

        /// <summary>
        /// 請求參數-頁面第幾頁
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// 請求參數-頁面顯示幾筆
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 請求參數-排序類別
        /// </summary>
        public int SortBy { get; set; } = 1;
    }
}