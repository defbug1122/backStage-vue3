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

        /// <summary>
        /// 當前登入用戶SessionId與資料庫不匹配
        /// </summary>
        UnMatchSessionId = 5,

        /// <summary>
        /// 權限不足
        /// </summary>
        PermissionDenied = 6,

        /// <summary>
        /// 資料庫無該會員資料
        /// </summary>
        NotFoundMember = 7,

        /// <summary>
        /// 用戶不存在
        /// </summary>
        NotFoundUser = 8
    }
}