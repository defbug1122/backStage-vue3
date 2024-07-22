using System.Collections.Generic;

namespace backStage_vue3.Models
{
    public class GetProductResponseDto
    {
        /// <summary>
        /// 回應狀態碼
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 回傳商品列表資訊
        /// </summary>
        public List<ProductModel> Data { get; set; }

        /// <summary>
        /// 回應是否還有下一頁
        /// </summary>
        public bool HasMore { get; set; }

        /// <summary>
        /// 商品安全庫存量
        /// </summary>
        public int SafetyStock { get; set; }
    }
}