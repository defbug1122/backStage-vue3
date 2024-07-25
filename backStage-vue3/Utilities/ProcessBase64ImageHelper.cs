using System;
using System.IO;
using System.Web;

namespace backStage_vue3.Utilities
{
    public static class ProcessBase64ImageHelper
    {
        /// <summary>
        /// 處理商品Base64圖片
        /// </summary>
        /// <param name="base64Images"></param>
        /// <returns></returns>
        public static string[] ProcessBase64Images(string[] base64Images, int maxFileSizeInBytes)
        {
            string[] imagePaths = new string[base64Images.Length];

            for (int i = 0; i < base64Images.Length; i++)
            {
                var base64Image = base64Images[i];

                // 空字串代表不更新資料庫圖片
                if (base64Image == "")
                {
                    imagePaths[i] = "";
                }
                else if (!string.IsNullOrEmpty(base64Image))
                {
                    // 分為檔案格式、base64碼
                    var imageParts = base64Image.Split(',');

                    // 驗證圖片格式、檔案大小
                    ImageValidator.ValidateBase64Image(imageParts, maxFileSizeInBytes);

                    // 檔案格式
                    var header = imageParts[0];

                    // base64碼
                    var data = imageParts[1];
                    var fileType = header.Split(';')[0].Split('/')[1];

                    // 將base64碼 轉換成圖像
                    var bytes = Convert.FromBase64String(data);
                    var fileExtension = fileType;
                    var newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + (i + 1) + "." + fileExtension;
                    var uploadsDir = HttpContext.Current.Server.MapPath("~/Uploads");

                    // 確保目錄存在
                    if (!Directory.Exists(uploadsDir))
                    {
                        Directory.CreateDirectory(uploadsDir);
                    }

                    var filePath = Path.Combine(uploadsDir, newFileName);

                    // 將圖檔寫入資料夾
                    File.WriteAllBytes(filePath, bytes);
                    imagePaths[i] = $"{newFileName}";
                }
            }

            return imagePaths;
        }
    }
}