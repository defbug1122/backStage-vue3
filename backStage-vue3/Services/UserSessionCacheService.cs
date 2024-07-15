using backStage_vue3.Models;
using System;
using System.Runtime.Caching;

namespace backStage_vue3.Services
{
    /// <summary>
    /// 用戶會話緩存
    /// </summary>
    public class UserSessionCacheService
    {
        /// <summary>
        /// 創立一個緩存實例只能在內部被讀取，名為 UserSessionCache
        /// </summary>
        private static readonly MemoryCache UserSessionCache = new MemoryCache("UserSessionCache");

        /// <summary>
        /// 創立一個私有單一的 key，不能被修改的 key 
        /// </summary>
        private static readonly object CacheLock = new object();

        /// <summary>
        /// 添加用戶會話到緩存
        /// </summary>
        /// <param name="session"></param>
        public void AddUserSession(UserSessionModel session)
        {
            var cacheItemPolicy = new CacheItemPolicy
            {
                SlidingExpiration = TimeSpan.FromMinutes(20) // 設置緩存時間，20分鐘沒有活動會過期
            };

            // 進入lock 區塊，會去檢查此 key 是否被佔用，如果被佔用，就要排隊等待
            lock (CacheLock)
            {
                UserSessionCache.Set(session.Id.ToString(), session, cacheItemPolicy);
            }
        }

        /// <summary>
        /// 從緩存中獲取用戶會話
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserSessionModel GetUserSession(int userId)
        {
            return UserSessionCache.Get(userId.ToString()) as UserSessionModel;
        }

        /// <summary>
        /// 清理用戶會話緩存
        /// </summary>
        /// <param name="userId"></param>
        public void RemoveUserSession(int userId)
        {
            lock (CacheLock)
            {
                UserSessionCache.Remove(userId.ToString());
            }
        }

        /// <summary>
        /// 從緩存中更新用戶權限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newPermission"></param>
        public void UpdateUserPermission(int userId, int newPermission)
        {
            lock (CacheLock)
            {
                // 首先先確認該用戶是否在緩存中
                var session = UserSessionCache.Get(userId.ToString()) as UserSessionModel;
                if (session != null)
                {
                    // 創建一個新的 UserSessionModel 實例來更新該用戶會話緩存
                    var updatedSession = new UserSessionModel
                    {
                        Id = session.Id,
                        UserName = session.UserName,
                        Permission = newPermission,
                        SessionID = session.SessionID
                    };

                    UserSessionCache.Set(updatedSession.Id.ToString(), updatedSession, new CacheItemPolicy
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(20) // 設置緩存時間，20分鐘沒有活動會過期
                    });
                }
            }
        }
    }
}