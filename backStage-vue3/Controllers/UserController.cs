using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using backStage_vue3.Models;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System.Web;
using backStage_vue3.Utilities;

namespace backStage_vue3.Controllers
{
    public class UserController : ApiController
    {
        string sessionID = HttpContext.Current.Session.SessionID;
        
        private UserSessionModel GetCurrentUserSession()
        {
            return HttpContext.Current.Session["userSessionInfo"] as UserSessionModel;
        }

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
            var userSession = GetCurrentUserSession();

            if (string.IsNullOrEmpty(sessionID) || userSession == null)
            {
                return StatusCode(HttpStatusCode.Unauthorized);
            }

            string currentUn = userSession.CurrentUser;

            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(SqlConfig.conStr);
                connection.Open();
                using (SqlCommand checkCommand = new SqlCommand("pro_bs_getUuidByUn", connection))
                {
                    checkCommand.CommandType = CommandType.StoredProcedure;
                    checkCommand.Parameters.AddWithValue("@currentUn", currentUn);

                    var result = await checkCommand.ExecuteScalarAsync();
                    if (result == null || result.ToString() != sessionID)
                    {
                        return StatusCode(HttpStatusCode.Unauthorized);
                    }
                }

                using (SqlCommand command = new SqlCommand("pro_bs_getAllAcc", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@searchTerm", searchTerm);
                    command.Parameters.AddWithValue("@pageNumber", pageNumber);
                    command.Parameters.AddWithValue("@pageSize", pageSize);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        List<UserModel> users = new List<UserModel>();
                        while (reader.Read())
                        {
                            UserModel user = new UserModel
                            {
                                Id = Convert.ToInt32(reader["f_id"]),
                                Un = reader["f_un"].ToString(),
                                CreateTime = reader["f_createTime"] != DBNull.Value ? Convert.ToDateTime(reader["f_createTime"]) : DateTime.MinValue,
                                Permission = reader["f_Permission"].ToString(),
                            };
                            users.Add(user);
                        }

                        reader.NextResult();

                        int totalRecords = 0;
                        if (reader.Read())
                        {
                            totalRecords = Convert.ToInt32(reader["TotalRecords"]);
                        }

                        var lowercaseUsers = users.Select(u => new
                        {
                            id = u.Id,
                            un = u.Un.ToLower(),
                            createTime = u.CreateTime?.ToString("yyyy-MM-dd"),
                            permission = u.Permission
                        });

                        bool hasMore = (pageNumber * pageSize) < totalRecords;

                        return Ok(new { code = StatusResCode.Success, data = lowercaseUsers, hasMore = hasMore });
                    }
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
        /// HTTP POST 用户登入 API
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/user/login")]
        public async Task<IHttpActionResult> Login(UserLoginModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string pattern = @"^[a-zA-Z0-9_-]{4,16}$";

            if (!System.Text.RegularExpressions.Regex.IsMatch(model.Un, pattern) ||
                !System.Text.RegularExpressions.Regex.IsMatch(model.Pwd, pattern))
            {
                return Ok(new { code = StatusResCode.InvalidFormat });
            }

            string hashPwd = HashHelper.ComputeSha256Hash(model.Pwd);

            using (SqlConnection connection = new SqlConnection(SqlConfig.conStr))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("pro_bs_getUserLogin", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@un", model.Un);
                        command.Parameters.AddWithValue("@pwd", hashPwd);
                        command.Parameters.AddWithValue("@uuid", HttpContext.Current.Session.SessionID);
                        SqlParameter statusCodeParam = new SqlParameter("@statusCode", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(statusCodeParam);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);

                            int statusCode = (int)statusCodeParam.Value;
                            if (statusCode == 0 && dataTable.Rows.Count > 0)
                            {
                                string userId = Convert.ToString(dataTable.Rows[0]["f_id"]);
                                string permission = Convert.ToString(dataTable.Rows[0]["f_permission"]);

                                UserSessionModel userSession = new UserSessionModel
                                {
                                    CurrentUser = model.Un,
                                    CurrentPermission = permission
                                };

                                HttpContext.Current.Session["userSessionInfo"] = userSession;

                                HttpCookie uuidCookie = new HttpCookie("uuid", HttpContext.Current.Session.SessionID);
                                HttpCookie permissionCookie = new HttpCookie("permission", permission);
                                HttpCookie currentUserCookie = new HttpCookie("currentUser", model.Un);

                                HttpContext.Current.Response.Cookies.Add(uuidCookie);
                                HttpContext.Current.Response.Cookies.Add(permissionCookie);
                                HttpContext.Current.Response.Cookies.Add(currentUserCookie);

                                return Ok(new { code = StatusResCode.Success });
                            }
                            else
                            {
                                return Ok(new { code = StatusResCode.Failed });
                            }
                        }
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

        }

        /// <summary>
        /// HTTP POST 新增用戶 API
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/user/add")]
        public async Task<IHttpActionResult> CreateUser(UserAddModel model)
        {

            var userSession = GetCurrentUserSession();

            if (string.IsNullOrEmpty(sessionID) || userSession == null)
            {
                return StatusCode(HttpStatusCode.Unauthorized);
            }

            model.CurrentUser = userSession.CurrentUser;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string pattern = @"^[a-zA-Z0-9_-]{4,16}$";

            if (
                !System.Text.RegularExpressions.Regex.IsMatch(model.Un, pattern) ||
                !System.Text.RegularExpressions.Regex.IsMatch(model.Pwd, pattern))
            {
                return Ok(new { code = StatusResCode.InvalidFormat });
            }

            SqlConnection connection = null;

            string hashPwd = HashHelper.ComputeSha256Hash(model.Pwd);

            try
            {
                connection = new SqlConnection(SqlConfig.conStr);
                connection.Open();

                // 驗證 sessionID 是否與資料庫中的 f_uuid 匹配
                using (SqlCommand checkCommand = new SqlCommand("pro_bs_getUuidByUn", connection))
                {
                    checkCommand.CommandType = CommandType.StoredProcedure;
                    checkCommand.Parameters.AddWithValue("@currentUn", model.CurrentUser);

                    var result = await checkCommand.ExecuteScalarAsync();
                    if (result == null || result.ToString() != sessionID)
                    {
                        return StatusCode(HttpStatusCode.Unauthorized);
                    }
                }

                // 繼續新增用戶
                using (SqlCommand command = new SqlCommand("pro_bs_addNewUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@un", model.Un);
                    command.Parameters.AddWithValue("@pwd", hashPwd);
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
                        return Ok(new { code = StatusResCode.Success });
                    }
                    else
                    {
                        return Ok(new { code = StatusResCode.Failed });
                    }
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
        /// HTTP POST 刪除用戶 API
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/user/delete")]
        public async Task<IHttpActionResult> DeleteUser(UserDeleteModel model)
        {

            var userSession = GetCurrentUserSession();

            if (string.IsNullOrEmpty(sessionID) || userSession == null)
            {
                return StatusCode(HttpStatusCode.Unauthorized);
            }

            model.CurrentUser = userSession.CurrentUser;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (model.CurrentUser == model.Un)
            {
                return Ok(new { code = StatusResCode.DeleteMyself });
            }

            SqlConnection connection = null;

            try
            {

                connection = new SqlConnection(SqlConfig.conStr);
                connection.Open();
                // 驗證 sessionID 是否與資料庫中的 f_uuid 匹配
                using (SqlCommand checkCommand = new SqlCommand("pro_bs_getUuidByUn", connection))
                {
                    checkCommand.CommandType = CommandType.StoredProcedure;
                    checkCommand.Parameters.AddWithValue("@currentUn", model.CurrentUser);

                    var result = await checkCommand.ExecuteScalarAsync();
                    if (result == null || result.ToString() != sessionID)
                    {
                        return StatusCode(HttpStatusCode.Unauthorized);
                    }
                }

                // 刪除用戶
                using (SqlCommand command = new SqlCommand("pro_bs_delUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@un", model.Un);
                    command.Parameters.AddWithValue("@currentUn", model.CurrentUser);

                    SqlParameter statusCodeParam = new SqlParameter("@statusCode", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(statusCodeParam);

                    await command.ExecuteNonQueryAsync();

                    int statusCode = (int)statusCodeParam.Value;
                    if (statusCode == 0)
                    {
                        return Ok(new { code = StatusResCode.Success });
                    }
                    else
                    {
                        return Ok(new { code = StatusResCode.Failed });
                    }
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
        /// HTTP POST 更新用戶 API
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/user/update")]
        public async Task<IHttpActionResult> EditUser(UserUpdateModel model)
        {

            var userSession = GetCurrentUserSession();

            if (string.IsNullOrEmpty(sessionID) || userSession == null)
            {
                return StatusCode(HttpStatusCode.Unauthorized);
            }

            model.CurrentUser = userSession.CurrentUser;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string pattern = @"^[a-zA-Z0-9_-]{4,16}$";

            if (
                !System.Text.RegularExpressions.Regex.IsMatch(model.Un, pattern) ||
                !System.Text.RegularExpressions.Regex.IsMatch(model.Pwd, pattern))
            {
                return Ok(new { code = StatusResCode.InvalidFormat });
            }

            string hashPwd = HashHelper.ComputeSha256Hash(model.Pwd);

            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(SqlConfig.conStr);
                connection.Open();

                // 驗證 sessionID 是否與資料庫中的 f_uuid 匹配
                using (SqlCommand checkCommand = new SqlCommand("pro_bs_getUuidByUn", connection))
                {
                    checkCommand.CommandType = CommandType.StoredProcedure;
                    checkCommand.Parameters.AddWithValue("@currentUn", model.CurrentUser);

                    var result = await checkCommand.ExecuteScalarAsync();
                    if (result == null || result.ToString() != sessionID)
                    {
                        return StatusCode(HttpStatusCode.Unauthorized);
                    }
                }

                // 更新用戶
                using (SqlCommand command = new SqlCommand("pro_bs_editUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@un", model.Un);
                    command.Parameters.AddWithValue("@newPwd", hashPwd);
                    command.Parameters.AddWithValue("@newPermission", model.Permission);
                    command.Parameters.AddWithValue("@currentUn", model.CurrentUser);
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
                        return Ok(new { code = StatusResCode.Success });
                    }
                    else
                    {
                        return Ok(new { code = StatusResCode.Failed });
                    }
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
    }
}