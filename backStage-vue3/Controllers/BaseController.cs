using backStage_vue3.Filter;
using backStage_vue3.Models;
using System.Web;
using System.Web.Http;

namespace backStage_vue3.Controllers
{
    [SessionAuthorizationFilter]
    public class BaseController : ApiController
    {
        protected UserSessionModel UserSession { get; private set; }

        public BaseController()
        {
            UserSession = HttpContext.Current.Session["userSessionInfo"] as UserSessionModel;
        }
    }
}