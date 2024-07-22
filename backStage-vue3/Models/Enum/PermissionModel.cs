namespace backStage_vue3.Models
{
    /// <summary>
    /// 權限定義
    /// </summary>
    public enum Permissions
    {

        /// <summary>
        /// 超級權限
        /// </summary>
        SuperPermission = 1,

        /// <summary>
        /// 新增帳號
        /// </summary>
        AddAccount = 2,

        /// <summary>
        /// 刪除帳號
        /// </summary>
        DeleteAccount = 4,

        /// <summary>
        /// 編輯帳號
        /// </summary>
        EditAccount = 8,

        /// <summary>
        /// 查看帳號 
        /// </summary>
        ViewAccount = 16,

        /// <summary>
        /// 查看會員
        /// </summary>
        ViewMember = 32,

        /// <summary>
        /// 設置會員等級
        /// </summary>
        SetMemberLevel = 64,

        /// <summary>
        /// 設置會員停權
        /// </summary>
        SuspendMember = 128,

        /// <summary>
        /// 新增商品
        /// </summary>
        AddProduct = 256,

        /// <summary>
        /// 查看商品
        /// </summary>
        ViewProduct = 512,

        /// <summary>
        /// 編輯商品
        /// </summary>
        EditProduct = 1024,

        /// <summary>
        /// 刪除商品
        /// </summary>
        DeleteProduct = 2048,

        /// <summary>
        /// 查看訂單
        /// </summary>
        ViewOrder = 4096,

        /// <summary>
        /// 編輯訂單
        /// </summary>
        EditOrder = 8192,

        /// <summary>
        /// 刪除訂單
        /// </summary>
        DeleteOrder = 16384,
    }
}