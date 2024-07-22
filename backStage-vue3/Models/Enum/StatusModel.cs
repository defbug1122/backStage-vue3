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
        /// 缺少正確請求參數
        /// </summary>
        MissingParams = 3,

        /// <summary>
        /// 不能刪除自己
        /// </summary>
        DeleteMyself = 4,

        /// <summary>
        /// 當前登入用戶SessionId或權限與資料庫不匹配
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
        NotFoundUser = 8,

        /// <summary>
        /// 不能修改自己權限
        /// </summary>
        CannotEditOwnPermission = 9,

        /// <summary>
        /// 沒有修改權限
        /// </summary>
        CannotModifyPermission = 10,

        /// <summary>
        /// 權限設定失敗
        /// </summary>
        SetPermissionFailed = 11,

        /// <summary>
        /// 權限被異動
        /// </summary>
        PermissionChange = 12,

        /// <summary>
        /// 缺少身分驗證
        /// </summary>
        MissingAuthentication = 13,

        /// <summary>
        /// 找不到該項商品
        /// </summary>
        NotFoundProduct = 14,

        /// <summary>
        /// 圖片檔案太大
        /// </summary>
        ImageFileIsLarge = 15,

        /// <summary>
        /// 圖片上傳格式錯誤
        /// </summary>
        ImageFormatError = 16,

        /// <summary>
        /// 商品缺少封面圖片
        /// </summary>
        MissingCoverImage = 17
    }
}