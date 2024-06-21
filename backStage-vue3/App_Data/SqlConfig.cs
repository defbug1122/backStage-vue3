using System;
using System.Configuration;

namespace backStage_vue3
{
    /// <summary>
    /// SQL 連線字串
    /// </summary>
    public class SqlConfig
    {
        public static String conStr = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
    }
}