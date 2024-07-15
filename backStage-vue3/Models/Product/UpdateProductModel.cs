using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backStage_vue3.Models
{
    /// <summary>
    /// 編輯商品 POST 請求
    /// </summary>
    public class UpdateProductModel
    {
        /// <summary>
        /// 編輯商品-商品編號
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// 編輯商品-商品名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 編輯商品-商品圖片1
        /// </summary>
        public string ImagePath1 { get; set; }

        /// <summary>
        /// 編輯商品-商品圖片2
        /// </summary>
        public string ImagePath2 { get; set; }

        /// <summary>
        /// 編輯商品-商品圖片3
        /// </summary>
        public string ImagePath3 { get; set; }

        /// <summary>
        /// 編輯商品-商品類型
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// 編輯商品-商品價格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 編輯商品-商品是否開放
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// 編輯商品-商品描述
        /// </summary>
        public string Describe { get; set; }

        /// <summary>
        /// 編輯商品-商品庫存
        /// </summary>
        public int Stock { get; set; }
    }
}