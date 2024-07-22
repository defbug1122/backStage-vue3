using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using backStage_vue3.Models;
using backStage_vue3.Utilities;

namespace backStage_vue3.Filter
{
    public class GlobalExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            // 记录异常信息
            LoggerHelper.Log(LogLevel.Error, $"Unhandled exception: {context.Exception.Message}");

            // 返回统一的错误响应
            context.Response = context.Request.CreateResponse(HttpStatusCode.InternalServerError, new
            {
                code = 500,
                message = "Internal Server Error",
                detail = context.Exception.Message // 或者可以隐藏详细错误信息
            });
        }
    }
}