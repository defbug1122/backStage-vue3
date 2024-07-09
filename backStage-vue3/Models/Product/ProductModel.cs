using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backStage_vue3.Models
{
    /// <summary>
    /// 商品列表資訊
    /// </summary>
    public class ProductModel
    {
        /// <summary>
        /// 商品列表-商品編號
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// 商品列表-商品名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 商品列表-商品圖片
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// 商品列表-商品類型
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// 商品列表-商品價格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 商品列表-商品是否開放
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// 商品列表-商品描述
        /// </summary>
        public string Describe { get; set; }

        /// <summary>
        /// 商品列表-商品庫存
        /// </summary>
        public int Stock { get; set; }
    }

    /// <summary>
    /// 商品列表請求參數
    /// </summary>
    public class GetProdctRequest
    {
        /// <summary>
        /// 商品列表-關鍵字
        /// </summary>
        public string SearchTerm { get; set; } = "";

        /// <summary>
        /// 商品列表-頁面第幾頁
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// 商品列表-頁面顯示幾筆
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 商品列表-排序類別
        /// </summary>
        public int SortBy { get; set; } = 1;
    }
}