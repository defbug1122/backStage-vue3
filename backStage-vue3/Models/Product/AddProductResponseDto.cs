using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backStage_vue3.Models
{
    /// <summary>
    /// 新增商品 POST 回應
    /// </summary>
    public class AddProductResponseDto
    {
        /// <summary>
        /// 回應狀態碼
        /// </summary>
        public int Code { get; set; }
    }
}