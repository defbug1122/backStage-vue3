using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using backStage_vue3.Models;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using backStage_vue3.Utilities;
using System.Web;
using backStage_vue3.Services;

namespace backStage_vue3.Controllers
{
    public class UserController : BaseController
    {
        private readonly UserSessionCacheService _sessionService = new UserSessionCacheService();
        string pattern = @"^[a-zA-Z0-9_-]{4,16}$";

        /// <summary>
        /// HTTP GET 取得用戶列表 API
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet, Route("api/user/list")]
        public async Task<IHttpActionResult> GetUsers([FromUri] GetUserRequest model)
        {
            var result = new GetUserResponseDto();

            bool checkPermission = (UserSession.Permission & (int)Permissions.ViewAccount) == (int)Permissions.ViewAccount;

            if (!checkPermission)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            if (model == null)
            {
                result.Code = (int)StatusResCode.MissingParams;
                return Ok(result);
            }

            // 關鍵字搜尋不能超過16個字、頁數必須大於1、筆數只能查詢剛好10筆
            if ((model.SearchTerm != null && model.SearchTerm.Length > 16) || model.PageNumber <= 0 || model.PageSize != 10)
            {
                result.Code = (int)StatusResCode.InvalidFormat;
                return Ok(result);
            }

            SqlConnection connection = null;
            SqlCommand command = null;
            SqlDataReader reader = null;

            try
            {
                connection = new SqlConnection(SqlConfig.conStr);
                await connection.OpenAsync();
                command = new SqlCommand("pro_bs_getAllAcc", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@searchTerm", model.SearchTerm);
                command.Parameters.AddWithValue("@pageNumber", model.PageNumber);
                command.Parameters.AddWithValue("@pageSize", model.PageSize);
                command.Parameters.AddWithValue("@sortBy", model.SortBy);

                SqlParameter statusCodeParam = new SqlParameter("@statusCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(statusCodeParam);

                await command.ExecuteNonQueryAsync();

                int statusCode = (int)statusCodeParam.Value;

                if (statusCode == (int)StatusResCode.UnMatchSessionId)
                {
                    return StatusCode(HttpStatusCode.Unauthorized);
                }

                reader = await command.ExecuteReaderAsync();

                List<UserModel> users = new List<UserModel>();

                int totalRecords = 0;

                while (await reader.ReadAsync())
                {
                    UserModel user = new UserModel
                    {
                        Id = Convert.ToInt32(reader["f_userId"]),
                        UserName = reader["f_userName"].ToString(),
                        CreateTime = Convert.ToDateTime(reader["f_createTime"]).ToString("yyyy-MM-dd"),
                        Permission = Convert.ToInt32(reader["f_permission"]),
                    };
                    users.Add(user);
                    totalRecords = Convert.ToInt32(reader["TotalRecords"]);
                }

                bool hasMore = (model.PageNumber * model.PageSize) < totalRecords;

                result.Code = (int)StatusResCode.Success;
                result.Data = users;
                result.HasMore = hasMore;

                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    command.Parameters.Clear();
                }
                if (reader != null)
                {
                    reader.Close();
                }
            }

        }

        /// <summary>
        /// HTTP POST 新增用戶 API
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/user/add")]
        public async Task<IHttpActionResult> CreateUser(UserAddModel model)
        {
            var result = new AddUserResponseDto();
            bool checkPermission = (UserSession.Permission & (int)Permissions.AddAccount) == (int)Permissions.AddAccount;

            if (!checkPermission)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            if (model.UserName == null || model.Pwd == null || model.Permission == null)
            {
                result.Code = (int)StatusResCode.MissingParams;
                return Ok(result);
            }

            if (
                !System.Text.RegularExpressions.Regex.IsMatch(model.UserName, pattern) ||
                !System.Text.RegularExpressions.Regex.IsMatch(model.Pwd, pattern))
            {
                result.Code = (int)StatusResCode.InvalidFormat;
                return Ok(result);
            }

            // 檢查 model.Permission 不能為空，並且不能高於最大等級
            if (model.Permission == 0 || model.Permission > (int)Permissions.MaxPermission || (model.Permission & (int)Permissions.SuperPermission) == (int)Permissions.SuperPermission)
            {
                result.Code = (int)StatusResCode.SetPermissionFailed;
                return Ok(result);
            }

            // 判斷是否為超級管理員
            bool isSuperAdmin = (UserSession.Permission & (int)Permissions.SuperPermission) == (int)Permissions.SuperPermission;

            // 如果不是超級管理員，則進行其他權限檢查
            if (!isSuperAdmin)
            {
                bool hasAddPermission = (UserSession.Permission & (int)Permissions.AddAccount) == (int)Permissions.AddAccount;

                // 沒有新增帳號權限
                if (!hasAddPermission)
                {
                    result.Code = (int)StatusResCode.CannotModifyPermission;
                    return Ok(result);
                }

                // 有新增帳號權限，但不能新增自己的權限
                if (UserSession.Id == model.Id)
                {
                    result.Code = (int)StatusResCode.CannotEditOwnPermission;
                    return Ok(result);
                }

                // 有新增帳號權限，但不能設定其他人的修改、新增、刪除帳號權限
                if ((model.Permission & (int)(Permissions.EditAccount | Permissions.AddAccount | Permissions.DeleteAccount)) != 0)
                {
                    result.Code = (int)StatusResCode.SetPermissionFailed;
                    return Ok(result);
                }
            }

            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                connection = new SqlConnection(SqlConfig.conStr);
                connection.Open();
                command = new SqlCommand("pro_bs_addNewUser", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@userName", model.UserName);
                command.Parameters.AddWithValue("@pwd", HashHelper.ComputeSha256Hash(model.Pwd));
                command.Parameters.AddWithValue("@createTime", DateTime.Now);
                command.Parameters.AddWithValue("@permission", model.Permission);

                SqlParameter statusCodeParam = new SqlParameter("@statusCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(statusCodeParam);

                await command.ExecuteNonQueryAsync();

                int statusCode = (int)statusCodeParam.Value;

                if (statusCode == (int)StatusResCode.Success)
                {
                    result.Code = (int)StatusResCode.Success;
                    return Ok(result);
                }
                else if (statusCode == (int)StatusResCode.UnMatchSessionId)
                {
                    return StatusCode(HttpStatusCode.Unauthorized);
                }
                else
                {
                    result.Code = (int)StatusResCode.Failed;
                    return Ok(result);
                }

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    command.Parameters.Clear();
                }
            }

        }

        /// <summary>
        /// HTTP POST 刪除用戶 API
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/user/delete")]
        public async Task<IHttpActionResult> DeleteUser(UserDeleteModel model)
        {
            var result = new DeleteUserResponseDto();
            bool checkPermission = (UserSession.Permission & (int)Permissions.DeleteAccount) == (int)Permissions.DeleteAccount;

            if (!checkPermission)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            if (model.Id == null)
            {
                result.Code = (int)StatusResCode.MissingParams;
                return Ok(result);
            }

            if (UserSession.Id == model.Id)
            {
                result.Code = (int)StatusResCode.DeleteMyself;
                return Ok(result);
            }

            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {

                connection = new SqlConnection(SqlConfig.conStr);
                connection.Open();
                command = new SqlCommand("pro_bs_delUser", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@userId", model.Id);

                SqlParameter statusCodeParam = new SqlParameter("@statusCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(statusCodeParam);

                await command.ExecuteNonQueryAsync();

                int statusCode = (int)statusCodeParam.Value;

                if (statusCode == (int)StatusResCode.Success)
                {
                    result.Code = (int)StatusResCode.Success;
                    return Ok(result);
                }
                else if (statusCode == (int)StatusResCode.UnMatchSessionId)
                {
                    return StatusCode(HttpStatusCode.Unauthorized);
                }
                else
                {
                    result.Code = (int)StatusResCode.Failed;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    command.Parameters.Clear();
                }
            }

        }

        /// <summary>
        /// HTTP POST 更新用戶權限 API
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/user/updateRole")]
        public async Task<IHttpActionResult> EditRole(UserEditRoleModel model)
        {
            var result = new UserEditRoleResponseDto();

            if (model.Id == 0 || model.Permission == 0)
            {
                result.Code = (int)StatusResCode.MissingParams;
                return Ok(result);
            }

            // 檢查 model.Permission 不能為空，並且不能高於最大權限
            if (model.Permission > 32766 || (model.Permission & (int)Permissions.SuperPermission) == (int)Permissions.SuperPermission)
            {
                result.Code = (int)StatusResCode.SetPermissionFailed;
                return Ok(result);
            }

            // 判斷是否為超級管理員
            bool isSuperAdmin = (UserSession.Permission & (int)Permissions.SuperPermission) == (int)Permissions.SuperPermission;

            // 如果不是超級管理員，則進行其他權限檢查
            if (!isSuperAdmin)
            {
                bool hasEditPermission = (UserSession.Permission & (int)Permissions.EditAccount) == (int)Permissions.EditAccount;

                // 沒有修改帳號權限
                if (!hasEditPermission)
                {
                    result.Code = (int)StatusResCode.CannotModifyPermission;
                    return Ok(result);
                }

                // 有修改帳號權限，但不能修改自己的權限
                if (UserSession.Id == model.Id)
                {
                    result.Code = (int)StatusResCode.CannotEditOwnPermission;
                    return Ok(result);
                }

                // 有修改帳號權限，但不能設定其他人的修改、新增、刪除帳號權限
                if ((model.Permission & (int)(Permissions.EditAccount | Permissions.AddAccount | Permissions.DeleteAccount)) != 0)
                {
                    result.Code = (int)StatusResCode.SetPermissionFailed;
                    return Ok(result);
                }
            }

            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                connection = new SqlConnection(SqlConfig.conStr);
                connection.Open();
                command = new SqlCommand("pro_bs_editUserRole", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@userId", model.Id);
                command.Parameters.AddWithValue("@newPermission", model.Permission);
                command.Parameters.AddWithValue("@updateTime", DateTime.Now);

                SqlParameter statusCodeParam = new SqlParameter("@statusCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(statusCodeParam);

                await command.ExecuteNonQueryAsync();

                int statusCode = (int)statusCodeParam.Value;

                if (statusCode == (int)StatusResCode.Success)
                {
                    _sessionService.UpdateUserPermission(model.Id, model.Permission);

                    // 強制更新當前會話信息，已確保權限更新生效
                    if (UserSession.Id == model.Id)
                    {
                        UserSession.Permission = model.Permission;
                        HttpContext.Current.Session["userSessionInfo"] = UserSession;
                    }

                    result.Code = (int)StatusResCode.Success;
                    return Ok(result);
                }
                else if (statusCode == (int)StatusResCode.UnMatchSessionId)
                {
                    return StatusCode(HttpStatusCode.Unauthorized);
                }
                else
                {
                    result.Code = (int)StatusResCode.Failed;
                    return Ok(result);
                }

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

        }

        /// <summary>
        /// HTTP POST 編輯用戶密碼 API
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/user/editInfo")]
        public async Task<IHttpActionResult> ResetInfo(UserEditInfoModel model)
        {
            var result = new UserEditInfoResponseDto();
            // 判斷是否具有修改帳號權限
            bool hasEditPermission = (UserSession.Permission & (int)Permissions.EditAccount) == (int)Permissions.EditAccount;

            // 如果嘗試修改其他人的密碼但沒有修改帳號權限，返回403禁止
            if (UserSession.Id != model.Id && !hasEditPermission)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            if (model.Id == 0 || model.Pwd == null)
            {
                result.Code = (int)StatusResCode.MissingParams;
                return Ok(result);
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(model.Pwd, pattern))
            {
                result.Code = (int)StatusResCode.InvalidFormat;
                return Ok(result);
            }

            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                connection = new SqlConnection(SqlConfig.conStr);
                connection.Open();
                command = new SqlCommand("pro_bs_editUserInfo", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@userId", model.Id);
                command.Parameters.AddWithValue("@newPwd", HashHelper.ComputeSha256Hash(model.Pwd));
                command.Parameters.AddWithValue("@updateTime", DateTime.Now);

                SqlParameter statusCodeParam = new SqlParameter("@statusCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(statusCodeParam);

                await command.ExecuteNonQueryAsync();

                int statusCode = (int)statusCodeParam.Value;

                if (statusCode == (int)StatusResCode.Success)
                {
                    result.Code = (int)StatusResCode.Success;
                    return Ok(result);
                }
                else if (statusCode == (int)StatusResCode.UnMatchSessionId)
                {
                    return StatusCode(HttpStatusCode.Unauthorized);
                }
                else
                {
                    result.Code = (int)StatusResCode.Failed;
                    return Ok(result);
                }

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

        }

        /// <summary>
        /// HTTP POST 用户登出 API
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/user/logout")]
        public async Task<IHttpActionResult> Logout()
        {
            var result = new UserLogoutResponseDto();

            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                connection = new SqlConnection(SqlConfig.conStr);
                await connection.OpenAsync();

                command = new SqlCommand("pro_bs_getUserLogout", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@currentUserId",UserSession.Id);
                SqlParameter statusCodeParam = new SqlParameter("@statusCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(statusCodeParam);

                await command.ExecuteNonQueryAsync();
                int statusCode = (int)statusCodeParam.Value;

                if (statusCode == (int)StatusResCode.Success)
                {
                    // 清理緩存
                    _sessionService.RemoveUserSession(UserSession.Id);

                    // 清理Session的值
                    HttpContext.Current.Session.Clear();

                    // 終止當前會話
                    HttpContext.Current.Session.Abandon();

                    // 創新的Cookies將其設為過期，瀏覽器接收到是過期的，便會清理該對應 Cookies
                    if (HttpContext.Current.Request.Cookies["uuid"] != null)
                    {
                        var uuidCookie = new HttpCookie("uuid")
                        {
                            Expires = DateTime.Now.AddDays(-1)
                        };
                        HttpContext.Current.Response.Cookies.Add(uuidCookie);
                    }

                    if (HttpContext.Current.Request.Cookies["permission"] != null)
                    {
                        var permissionCookie = new HttpCookie("permission")
                        {
                            Expires = DateTime.Now.AddDays(-1)
                        };
                        HttpContext.Current.Response.Cookies.Add(permissionCookie);
                    }

                    if (HttpContext.Current.Request.Cookies["currentUser"] != null)
                    {
                        var currentUserCookie = new HttpCookie("currentUser")
                        {
                            Expires = DateTime.Now.AddDays(-1)
                        };
                        HttpContext.Current.Response.Cookies.Add(currentUserCookie);
                    }

                    result.Code = (int)StatusResCode.Success;
                    return Ok(result);
                } else
                {
                    result.Code = (int)StatusResCode.Failed;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            finally
            {

                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    command.Parameters.Clear();
                }
            }
        }
    }
}