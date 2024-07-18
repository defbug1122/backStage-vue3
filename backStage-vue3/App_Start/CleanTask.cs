using backStage_vue3.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;
using backStage_vue3.Utilities;

namespace backStage_vue3.App_Start
{
    /// <summary>
    /// 清理任務
    /// </summary>
    public class CleanupTask
    {
        private Timer _timer;
        private readonly string _uploadPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Uploads"); // 目標資料夾
        private readonly string _connectionString = SqlConfig.conStr;
        private readonly int _maxRetries = 3; // 重試最多到3次
        private readonly TimeSpan _retryInterval = TimeSpan.FromMinutes(5); //重試間格為5分鐘

        /// <summary>
        /// 啟動任務
        /// </summary>
        public void Start()
        {
            //Logger.CleanupOldLogs(); // 清理過期日誌

            // 每周清理一次作業
            _timer = new Timer(ExecuteTask, null, TimeSpan.Zero, TimeSpan.FromDays(7));
            LoggerHelper.Log(LogLevel.Info,"啟動清理任務。");
        }

        /// <summary>
        /// 停止任務
        /// </summary>
        public void Stop()
        {
            _timer?.Change(Timeout.Infinite, 0); // 停止計時器
            LoggerHelper.Log(LogLevel.Info,"停止清理任務。");
        }

        /// <summary>
        /// 執行任務內容
        /// </summary>
        /// <param name="state"></param>
        private void ExecuteTask(object state)
        {
            int attempt = 0; // 重試次數

            while (attempt < _maxRetries)
            {
                try
                {
                    LoggerHelper.Log(LogLevel.Info, $"清理任務執行第 {attempt + 1} 次開始。");

                    var files = Directory.GetFiles(_uploadPath);
                    var validFiles = GetValidFilesFromDatabase(_connectionString);

                    foreach (var file in files)
                    {
                        string fileName = Path.GetFileName(file);
                        if (!validFiles.Contains(fileName))
                        {
                            File.Delete(file);
                            LoggerHelper.Log(LogLevel.Info, $"刪除: {fileName}");
                        }
                    }

                    LoggerHelper.Log(LogLevel.Info, "清理任務完成。");
                    break; // 任務執行成功完成，跳出迴圈
                }
                catch (Exception ex)
                {
                    attempt++;
                    LoggerHelper.Log(LogLevel.Error, $"重試 {attempt} 次時發生錯誤: {ex.Message}");

                    if (attempt < _maxRetries)
                    {
                        LoggerHelper.Log(LogLevel.Warning, $"將在 {_retryInterval.TotalMinutes} 分鐘後重試...");
                        Thread.Sleep(_retryInterval); // 等待5分鐘後重試
                    }
                    else
                    {
                        LoggerHelper.Log(LogLevel.Error, "已嘗試到最大重試次數. 任務執行失敗。");
                    }
                }
            }   


        }

        /// <summary>
        /// 從資料庫獲取目前還存在有效的圖片檔案
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private string[] GetValidFilesFromDatabase(string connectionString)
        {
            var validFiles = new List<string>(); // 目前有效圖片

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand("pro_bs_getProductImagePaths", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (!reader.IsDBNull(i))
                            {
                                validFiles.Add(reader.GetString(i));
                            }
                        }
                    }
                }
            }

            return validFiles.ToArray();
        }
    }
}
