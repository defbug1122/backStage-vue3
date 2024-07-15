using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backStage_vue3.Models
{
    /// <summary>
    /// 刪除商品 POST 回應參數
    /// </summary>
    public class DeleteProductResponseDto
    {
        /// <summary>
        /// 回應狀態碼
        /// </summary>
        public int Code { get; set; }
    }
}