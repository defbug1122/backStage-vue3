using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using backStage_vue3.Models;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using backStage_vue3.Utilities;
using System.Web;

namespace backStage_vue3.Controllers
{
    public class UserController : BaseController
    {
        /// <summary>
        /// HTTP GET 取得用戶列表 API
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet, Route("api/user/list")]
        public async Task<IHttpActionResult> GetUsers(string searchTerm = "", int pageNumber = 1, int pageSize = 10)
        {
            var result = new GetUserResponseDto();

            bool checkPermission = (UserSession.Permission & (int)Permissions.ViewAccount) == (int)Permissions.ViewAccount;

            if (!checkPermission)
            {
                return StatusCode(HttpStatusCode.Forbidden);
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

                //command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@currentUn", UserSession.Un);
                command.Parameters.AddWithValue("@currentSessionID", UserSession.SessionID);
                command.Parameters.AddWithValue("@searchTerm", searchTerm);
                command.Parameters.AddWithValue("@pageNumber", pageNumber);
                command.Parameters.AddWithValue("@pageSize", pageSize);

                SqlParameter statusCodeParam = new SqlParameter("@statusCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(statusCodeParam);

                await command.ExecuteNonQueryAsync();

                int statusCode = (int)statusCodeParam.Value;

                if (statusCode == 5)
                {
                    return StatusCode(HttpStatusCode.Unauthorized);
                }

                reader = await command.ExecuteReaderAsync();

                List<UserModel> users = new List<UserModel>();
               
                while (await reader.ReadAsync())
                {
                    UserModel user = new UserModel
                    {
                        Id = Convert.ToInt32(reader["f_id"]),
                        Un = reader["f_un"].ToString(),
                        CreateTime = Convert.ToDateTime(reader["f_createTime"]).ToString("yyyy-MM-dd"),
                        Permission = Convert.ToInt32(reader["f_permission"]),
                    };
                    users.Add(user);
                }

                // 移到下一个结果集，獲取總數
                await reader.NextResultAsync();

                int totalRecords = 0;

                if (await reader.ReadAsync())
                {
                    totalRecords = Convert.ToInt32(reader["TotalRecords"]);
                }

                bool hasMore = (pageNumber * pageSize) < totalRecords;

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

        string pattern = @"^[a-zA-Z0-9_-]{4,16}$";

        /// <summary>
        /// HTTP POST 新增用戶 API
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/user/add")]
        public async Task<IHttpActionResult> CreateUser(UserAddModel model)
        {
            bool checkPermission = (UserSession.Permission & (int)Permissions.AddAccount) == (int)Permissions.AddAccount;

            if (!checkPermission)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            var result = new AddUserResponseDto();

            if (
                !System.Text.RegularExpressions.Regex.IsMatch(model.Un, pattern) ||
                !System.Text.RegularExpressions.Regex.IsMatch(model.Pwd, pattern))
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
                command = new SqlCommand("pro_bs_addNewUser", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                //command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@currentUn", UserSession.Un);
                command.Parameters.AddWithValue("@currentSessionID", UserSession.SessionID);
                command.Parameters.AddWithValue("@un", model.Un);
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

                if (statusCode == 0)
                {
                    result.Code = (int)StatusResCode.Success;
                    return Ok(result);
                }
                else if (statusCode == 5)
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

            bool checkPermission = (UserSession.Permission & (int)Permissions.DeleteAccount) == (int)Permissions.DeleteAccount;

            if (!checkPermission)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            var result = new DeleteUserResponseDto();

            if (UserSession.Un == model.Un)
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
                //command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@currentUn", UserSession.Un);
                command.Parameters.AddWithValue("@currentSessionID", UserSession.SessionID);
                command.Parameters.AddWithValue("@un", model.Un);

                SqlParameter statusCodeParam = new SqlParameter("@statusCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(statusCodeParam);

                await command.ExecuteNonQueryAsync();

                int statusCode = (int)statusCodeParam.Value;

                if (statusCode == 0)
                {
                    result.Code = (int)StatusResCode.Success;
                    return Ok(result);
                }
                else if (statusCode == 5)
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
        /// HTTP POST 更新用戶 API
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/user/update")]
        public async Task<IHttpActionResult> EditUser(UserUpdateModel model)
        {
            bool checkPermission = (UserSession.Permission & (int)Permissions.EditAccount) == (int)Permissions.EditAccount;

            if (!checkPermission)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            var result = new UpdateUserResponseDto();

            if (
                !System.Text.RegularExpressions.Regex.IsMatch(model.Un, pattern) ||
                !System.Text.RegularExpressions.Regex.IsMatch(model.Pwd, pattern))
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
                command = new SqlCommand("pro_bs_editUser", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                //command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@currentUn", UserSession.Un);
                command.Parameters.AddWithValue("@currentSessionID", UserSession.SessionID);
                command.Parameters.AddWithValue("@un", model.Un);
                command.Parameters.AddWithValue("@newPwd", HashHelper.ComputeSha256Hash(model.Pwd));
                command.Parameters.AddWithValue("@newPermission", model.Permission);
                command.Parameters.AddWithValue("@updateTime", DateTime.Now);

                SqlParameter statusCodeParam = new SqlParameter("@statusCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(statusCodeParam);

                await command.ExecuteNonQueryAsync();

                int statusCode = (int)statusCodeParam.Value;

                if (statusCode == 0)
                {
                    result.Code = (int)StatusResCode.Success;
                    return Ok(result);
                }
                else if (statusCode == 5)
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
                command.Parameters.AddWithValue("@un",UserSession.Un);
                SqlParameter statusCodeParam = new SqlParameter("@statusCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(statusCodeParam);

                await command.ExecuteNonQueryAsync();
                int statusCode = (int)statusCodeParam.Value;

                if (statusCode == 0)
                {
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