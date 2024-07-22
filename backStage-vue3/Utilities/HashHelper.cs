using System.Security.Cryptography;
using System.Text;

namespace backStage_vue3.Utilities
{
    /// <summary>
    /// 雜湊處理
    /// </summary>
    public static class HashHelper
    {
        /// <summary>
        /// SHA256 加密
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ComputeSha256Hash(string data)
        {
            SHA256 sha256Hash = SHA256.Create();
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(data));
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }

            return builder.ToString();
        }   
    }
}