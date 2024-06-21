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
        /// <summary>
        /// 當前用戶Sessio資料
        /// </summary>
        /// <returns></returns>
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

            if (userSession == null)
            {
                return StatusCode(HttpStatusCode.Unauthorized);
            }

            string currentUn = userSession.CurrentUser;
            string currentSessionID = userSession.CurrentsessionID;

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

                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@currentUn", currentUn);
                command.Parameters.AddWithValue("@currentSessionID", currentSessionID);
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
                        CreateTime = reader["f_createTime"] != DBNull.Value ? Convert.ToDateTime(reader["f_createTime"]) : DateTime.MinValue,
                        Permission = reader["f_Permission"].ToString(),
                    };
                    users.Add(user);
                }

                await reader.NextResultAsync();

                int totalRecords = 0;

                if (await reader.ReadAsync())
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
            var userSession = GetCurrentUserSession();

            if (userSession == null)
            {
                return StatusCode(HttpStatusCode.Unauthorized);
            }

            model.CurrentUser = userSession.CurrentUser;
            model.CurrentsessionID = userSession.CurrentsessionID;

            string pattern = @"^[a-zA-Z0-9_-]{4,16}$";

            if (
                !System.Text.RegularExpressions.Regex.IsMatch(model.Un, pattern) ||
                !System.Text.RegularExpressions.Regex.IsMatch(model.Pwd, pattern))
            {
                return Ok(new { code = StatusResCode.InvalidFormat });
            }

            SqlConnection connection = null;
            SqlCommand command = null;

            string hashPwd = HashHelper.ComputeSha256Hash(model.Pwd);

            try
            {
                connection = new SqlConnection(SqlConfig.conStr);
                connection.Open();
                command = new SqlCommand("pro_bs_addNewUser", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@currentUn", model.CurrentUser);
                command.Parameters.AddWithValue("@currentSessionID", model.CurrentsessionID);
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
                else if (statusCode == 5)
                {
                    return StatusCode(HttpStatusCode.Unauthorized);
                }
                else
                {
                    return Ok(new { code = StatusResCode.Failed });
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

            var userSession = GetCurrentUserSession();

            if (userSession == null)
            {
                return StatusCode(HttpStatusCode.Unauthorized);
            }

            model.CurrentUser = userSession.CurrentUser;
            model.CurrentsessionID = userSession.CurrentsessionID;

            if (model.CurrentUser == model.Un)
            {
                return Ok(new { code = StatusResCode.DeleteMyself });
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
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@currentUn", model.CurrentUser);
                command.Parameters.AddWithValue("@currentSessionID", model.CurrentsessionID);
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
                    return Ok(new { code = StatusResCode.Success });
                }
                else if (statusCode == 5)
                {
                    return StatusCode(HttpStatusCode.Unauthorized);
                }
                else
                {
                    return Ok(new { code = StatusResCode.Failed });
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

            var userSession = GetCurrentUserSession();

            if (userSession == null)
            {
                return StatusCode(HttpStatusCode.Unauthorized);
            }

            model.CurrentUser = userSession.CurrentUser;
            model.CurrentsessionID = userSession.CurrentsessionID;

            string pattern = @"^[a-zA-Z0-9_-]{4,16}$";

            if (
                !System.Text.RegularExpressions.Regex.IsMatch(model.Un, pattern) ||
                !System.Text.RegularExpressions.Regex.IsMatch(model.Pwd, pattern))
            {
                return Ok(new { code = StatusResCode.InvalidFormat });
            }

            string hashPwd = HashHelper.ComputeSha256Hash(model.Pwd);

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
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@currentUn", model.CurrentUser);
                command.Parameters.AddWithValue("@currentSessionID", model.CurrentsessionID);
                command.Parameters.AddWithValue("@un", model.Un);
                command.Parameters.AddWithValue("@newPwd", hashPwd);
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
                    return Ok(new { code = StatusResCode.Success });
                }
                else if (statusCode == 5)
                {
                    return StatusCode(HttpStatusCode.Unauthorized);
                }
                else
                {
                    return Ok(new { code = StatusResCode.Failed });
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