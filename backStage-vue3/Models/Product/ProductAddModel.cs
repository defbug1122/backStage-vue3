namespace backStage_vue3.Models
{
    /// <summary>
    /// 新增商品資訊
    /// </summary>
    public class ProductAddModel
    {
        /// <summary>
        /// 商品列表-商品名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 商品列表-商品圖片1
        /// </summary>
        public string ImagePath1 { get; set; }

        /// <summary>
        /// 商品列表-商品圖片2
        /// </summary>
        public string ImagePath2 { get; set; }

        /// <summary>
        /// 商品列表-商品圖片3
        /// </summary>
        public string ImagePath3 { get; set; }

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
}