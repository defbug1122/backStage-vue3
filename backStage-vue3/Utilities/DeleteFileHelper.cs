using System.IO;
using System.Web;

namespace backStage_vue3.Utilities
{
    public static class DeleteFileHelper
    {
                /// <summary>
        /// 刪除圖片
        /// </summary>
        /// <param name="filePaths"></param>
        public static void DeleteFiles(string[] filePaths)
        {
            foreach (var path in filePaths)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    var filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads"), path);

                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
            }
        }
    }
}