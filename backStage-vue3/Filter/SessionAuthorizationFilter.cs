using backStage_vue3.Models;
using backStage_vue3.Services;
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
            var sessionService = new UserSessionCacheService();
            var cachedSession = sessionService.GetUserSession(session.Id);
            var result = new UserSessionResponseDto();

            if (session == null || cachedSession == null) {
                result.Code = (int)StatusResCode.MissingAuthentication;
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, result);
                return;
            }

            // 不重複登入判斷(後者踢前者) 
            if (session.SessionID != cachedSession.SessionID)
            {
                result.Code = (int)StatusResCode.UnMatchSessionId;
                HttpContext.Current.Session.Abandon();
                HttpContext.Current.Session.Clear();
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, result);
            }

            // 用戶權限被更改，須強制重新登入
            if (session.Permission != cachedSession.Permission)
            {
                result.Code = (int)StatusResCode.PermissionChange;
                HttpContext.Current.Session.Abandon();
                HttpContext.Current.Session.Clear();
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, result);
            }
        }
    }
}