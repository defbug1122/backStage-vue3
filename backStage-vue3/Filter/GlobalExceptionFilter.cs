using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using backStage_vue3.Models;
using backStage_vue3.Utilities;

namespace backStage_vue3.Filter
{
    /// <summary>
    /// 全局錯誤捕捉
    /// </summary>
    public class GlobalExceptionFilter : ExceptionFilterAttribute
    {
        /// <summary>
        /// 捕捉異常的訊息以及引發異常的請求和回應
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(HttpActionExecutedContext context)
        {
            // 紀錄異常訊息
            LoggerHelper.Log(LogLevel.Error, $"Unhandled exception: {context.Exception.Message}");

            // 返回统一500錯誤碼
            context.Response = context.Request.CreateResponse(HttpStatusCode.InternalServerError, new
            {
                code = 500,
                message = "Internal Server Error",
                detail = context.Exception.Message // 詳細錯誤訊息
            });
        }
    }
}