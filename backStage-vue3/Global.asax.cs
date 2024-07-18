using backStage_vue3.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace backStage_vue3
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private CleanupTask _cleanupTask;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // 啟動清理任務
            _cleanupTask = new CleanupTask();
            _cleanupTask.Start();
        }

        protected void Application_End()
        {
            // 確保應用程序停止時，停止任務繼續執行
            if (_cleanupTask != null)
            {
                _cleanupTask.Stop();
            }
        }

        protected void Application_PostAuthorizeRequest()
        {
            HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
        }
        protected void Session_start()
        {
            Session["user"] = User.Identity.Name;
            Session["count"] = 0;
        }
    }
}
