using System.Web.Mvc;

namespace backStage_vue3
{
    /// <summary>
    /// 註冊全局過濾器
    /// </summary>
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
