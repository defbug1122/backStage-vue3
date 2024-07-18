using backStage_vue3.Filter;
using backStage_vue3.Models;
using System.Web;
using System.Web.Http;

namespace backStage_vue3.Controllers
{
    // 身分驗證過濾器
    [SessionAuthorizationFilter]
    public class BaseController : ApiController
    {
        /// <summary>
        /// 初始化 UserSession
        /// </summary>
        protected UserSessionModel UserSession { get; set; }

        /// <summary>
        /// 繼承BaseController可取用UserSession
        /// </summary>
        public BaseController()
        {
            UserSession = HttpContext.Current.Session["userSessionInfo"] as UserSessionModel;
        }
    }
}