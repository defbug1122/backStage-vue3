using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backStage_vue3.Models
{
    public class GetProductResponseDto
    {
        /// <summary>
        /// 回應狀態碼
        /// </summary>
        public int Code { get; set; }

        public List<ProductModel> Data { get; set; }

        /// <summary>
        /// 回應是否還有下一頁
        /// </summary>
        public bool HasMore { get; set; }
    }
}