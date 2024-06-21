using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backStage_vue3.Models
{
    /// <summary>
    /// 狀態碼
    /// </summary>
    public enum StatusResCode
    {
        /// <summary>
        /// 操作成功
        /// </summary>
        Success = 0 ,

        /// <summary>
        /// 操作失敗
        /// </summary>
        Failed = 1,

        /// <summary>
        /// 請求參數格式錯誤
        /// </summary>
        InvalidFormat = 2,

        /// <summary>
        /// 缺少請求參數
        /// </summary>
        MissingParams = 3,

        /// <summary>
        /// 不能刪除自己
        /// </summary>
        DeleteMyself = 4,
    }
}