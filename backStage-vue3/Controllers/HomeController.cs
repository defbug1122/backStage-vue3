using System.Web.Mvc;

namespace backStage_vue3.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// 專案進入口
        /// </summary>
        public ActionResult Index()
        {
            return View();
        }
    }
}