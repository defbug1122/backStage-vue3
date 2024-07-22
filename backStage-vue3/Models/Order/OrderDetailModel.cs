using System.Collections.Generic;

namespace backStage_vue3.Models.Order
{
    /// <summary>
    /// 訂單明細回應
    /// </summary>
    public class OrderDetailResponse
    {
        /// <summary>
        /// 訂單明細-訂單流水號
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 訂單明細-訂單號碼
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// 訂單明細-訂單日期
        /// </summary>
        public string OrderDate { get; set; }

        /// <summary>
        /// 訂單明細-訂購會員名稱
        /// </summary>
        public string MemberName { get; set; }

        /// <summary>
        /// 訂單明細-收貨人
        /// </summary>
        public string Receiver { get; set; }

        /// <summary>
        /// 訂單明細-收貨人手機號碼
        /// </summary>
        public string MobileNumber { get; set; }

        /// <summary>
        /// 訂單明細-收貨地址
        /// </summary>
        public string DeliveryAddress { get; set; }

        /// <summary>
        /// 訂單明細-配送方式
        /// </summary>
        public byte DeliveryMethod { get; set; }

        /// <summary>
        /// 訂單明細-配送狀態
        /// </summary>
        public byte DeliveryStatus { get; set; }

        /// <summary>
        /// 訂單明細-訂單金額
        /// </summary>
        public decimal OrderAmount { get; set; }

        /// <summary>
        /// 訂單明細-訂單狀態
        /// </summary>
        public byte OrderStatus { get; set; }

        /// <summary>
        /// 訂單明細-退貨狀態
        /// </summary>
        public byte ReturnStatus { get; set; }

        /// <summary>
        /// 訂單明細-訂購項目
        /// </summary>
        public List<OrderDetailItemDto> SubtotalItems { get; set; }
    }

    /// <summary>
    /// 訂單明細-訂購項目
    /// </summary>
    public class OrderDetailItemDto
    {
        /// <summary>
        /// 訂購項目-訂單明細編號
        /// </summary>
        public int OrderDetailId { get; set; }

        /// <summary>
        /// 訂購項目-商品編號
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// 訂購項目-商品名稱
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 訂購項目-商品訂購數量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 訂購項目-商品價格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 訂購項目-商品總價格
        /// </summary>
        public decimal Subtotal { get; set; }
    }

    /// <summary>
    /// 訂單明細請求參數
    /// </summary>
    public class GetOrderDetailRequest
    {
        /// <summary>
        /// 請求參數-訂單流水號
        /// </summary>
        public int OrderId { get; set; }
    }
}