using backStage_vue3.Models;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace backStage_vue3.Filter
{
    /// <summary>
    /// 判斷當前用戶有沒有Session，才能繼續執行動作，否則身分驗證失敗返回Http Code 401
    /// </summary>
    public class SessionAuthorizationFilter : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var session = HttpContext.Current.Session["userSessionInfo"] as UserSessionModel;

            if (session == null) {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            }

        }
    }
}