using backStage_vue3.Models;
using System;
using System.IO;


namespace backStage_vue3.Utilities
{
    /// <summary>
    /// 日誌紀錄
    /// </summary>
    public static class LoggerHelper
    {
        private static readonly string LogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs"); // 目標資料夾
        private static readonly TimeSpan LogRetentionPeriod = TimeSpan.FromDays(180); // 日誌保留180天
        private static readonly object LockObject = new object(); // 鎖對象

        static LoggerHelper()
        {
            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
            }
        }

        /// <summary>
        /// 紀錄日誌內容
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public static void Log(LogLevel level, string message)
        {
            string logFilePath = Path.Combine(LogDirectory, $"{DateTime.Now:yyyy-MM-dd}.txt");
            string logMessage = $"{DateTime.Now}: [{level}] {message}{Environment.NewLine}";
            lock (LockObject)
            {
                File.AppendAllText(logFilePath, logMessage);
            }
        }

        /// <summary>
        /// 清除日誌檔案
        /// </summary>
        public static void CleanupOldLogs()
        {
            var logFiles = Directory.GetFiles(LogDirectory, "*.txt");

            foreach (var logFile in logFiles)
            {
                var lastWriteTime = File.GetLastWriteTime(logFile);

                if (DateTime.Now - lastWriteTime > LogRetentionPeriod)
                {
                    try
                    {
                        File.Delete(logFile);
                        Log(LogLevel.Info, $"刪除過期日誌文件: {logFile}");
                    }
                    catch (Exception ex)
                    {
                        Log(LogLevel.Error, $"刪除日誌文件 {logFile} 時發生錯誤: {ex.Message}");
                    }
                }
            }
        }
    }
}