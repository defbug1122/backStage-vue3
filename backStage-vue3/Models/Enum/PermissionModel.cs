
namespace backStage_vue3.Models
{
    public enum Permissions
    {
        /// <summary>
        /// 新增帳號
        /// </summary>
        AddAccount = 1,

        /// <summary>
        /// 刪除帳號
        /// </summary>
        DeleteAccount = 2,

        /// <summary>
        /// 編輯帳號
        /// </summary>
        EditAccount = 4,

        /// <summary>
        /// 查看帳號 8
        /// </summary>
        ViewAccount = 8,

        /// <summary>
        /// 查看會員
        /// </summary>
        ViewMember = 16,

        /// <summary>
        /// 設置會員等級
        /// </summary>
        SetMemberLevel = 32,

        /// <summary>
        /// 設置會員停權
        /// </summary>
        SuspendMember = 64,

        /// <summary>
        /// 新增商品
        /// </summary>
        AddProduct = 128,

        /// <summary>
        /// 查看商品
        /// </summary>
        ViewProduct = 256,

        /// <summary>
        /// 編輯商品
        /// </summary>
        EditProduct = 512,

        /// <summary>
        /// 刪除商品
        /// </summary>
        DeleteProduct = 1024,

        /// <summary>
        /// 查看訂單
        /// </summary>
        ViewOrder = 2048,

        /// <summary>
        /// 編輯訂單
        /// </summary>
        EditOrder = 4096,

        /// <summary>
        /// 刪除訂單
        /// </summary>
        DeleteOrder = 8192,
    }
}