using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backStage_vue3.Models.Enum
{
    /// <summary>
    /// 訂單配送狀態
    /// </summary>
    public enum OrderDeliveryStatusModel
    {
        /// <summary>
        /// 備貨中
        /// </summary>
        Preparing = 1,

        /// <summary>
        /// 已發貨
        /// </summary>
        Shipped = 2,

        /// <summary>
        /// 已送達
        /// </summary>
        Delivered = 3,

        /// <summary>
        /// 配送失敗
        /// </summary>
        DeliveryFailed = 4,

        /// <summary>
        /// 回收退貨中
        /// </summary>
        ReturnInProgress = 5,

        /// <summary>
        /// 已收到退貨
        /// </summary>
        ReturnReceived = 6,

        /// <summary>
        /// 店鋪準備商品中
        /// </summary>
        StorePreparing = 7,

        /// <summary>
        /// 待取貨
        /// </summary>
        ReadyForPickup = 8,

        /// <summary>
        /// 已取貨
        /// </summary>
        PickedUp = 9,

        /// <summary>
        /// 未取貨
        /// </summary>
        NotPickedUp = 10    
    }
}