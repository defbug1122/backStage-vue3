using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backStage_vue3.Utilities
{
    /// <summary>
    /// 圖片驗證
    /// </summary>
    public static class ImageValidator
    {
        /// <summary>
        /// 驗證圖片是否為Base64、格式、檔案大小
        /// </summary>
        /// <param name="imageParts"></param>
        /// <param name="maxFileSizeInBytes"></param>
        public static void ValidateBase64Image(string[] imageParts, int maxFileSizeInBytes)
        {
            // 確保Base 64 被分割為兩個部分(標頭、Base 64 數據)
            if (imageParts.Length != 2)
            {
                throw new ArgumentException("InvalidFormat");
            }

            var header = imageParts[0];
            var data = imageParts[1];
            var fileType = header.Split(';')[0].Split('/')[1];

            // 驗證是否為Base 64碼
            if(!IsBase64String(data))
            {
                throw new ArgumentException("InvalidBase64FormatError");
            }


            // 驗證圖片格式
            if (fileType != "jpg" && fileType != "jpeg" && fileType != "png")
            {
                throw new ArgumentException("ImageFormatError");
            }

            var bytes = Convert.FromBase64String(data);

            // 圖片檔案大小限制
            if (bytes.Length > maxFileSizeInBytes)
            {
                throw new ArgumentException("ImageFileIsLarge");
            }
        }

        /// <summary>
        /// 檢查字串是否為base64 編碼
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        private static bool IsBase64String(string base64)
        {
            if (string.IsNullOrEmpty(base64))
            {
                return false;
            }

            try
            {
                Convert.FromBase64String(base64);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

    }

}