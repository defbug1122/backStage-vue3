using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using backStage_vue3.Models;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Web.Http.Description;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;
using System.Web;

namespace backStage_vue3.Controllers
{
    public class UserController : ApiController
    {
        string conStr;
        public UserController()
        {
            conStr = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
        }

        public List<UserModel> ConnectToDatabase()
        {
            List<UserModel> users = new List<UserModel>();

            using (SqlConnection connection = new SqlConnection(conStr))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("pro_bs_getAllAccounts", connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        users = ConvertDataTableToList(dataTable);
                    }
                }
            }

            return users;
        }

        public List<UserModel> ConvertDataTableToList(DataTable dataTable)
        {
            List<UserModel> users = new List<UserModel>();

            foreach (DataRow row in dataTable.Rows)
            {
                UserModel user = new UserModel
                {
                    Id = Convert.ToInt32(row["f_id"]),
                    UserName = row["f_userName"].ToString(),
                    Password = row["f_password"].ToString(),
                    CreateTime = row["f_createTime"] != DBNull.Value ? Convert.ToDateTime(row["f_createTime"]) : DateTime.MinValue,
                    LoginTime = row["f_loginTime"] != DBNull.Value ? Convert.ToDateTime(row["f_loginTime"]) : DateTime.MinValue,
                    Permission = row["f_Permission"].ToString(),
                };
                users.Add(user);
            }

            return users;
        }

        // HTTP GET 取得用戶列表 API
        [HttpGet, Route("api/user/list")]
        [AllowAnonymous]
        [ResponseType(typeof(IEnumerable<UserModel>))]
        public async Task<IHttpActionResult> GetUsers(string searchTerm = "", int pageNumber = 1, int pageSize = 10)
        {
            string sessionID = HttpContext.Current.Session.SessionID;
            string currentUserName = HttpContext.Current.Session["currentUser"] as string;

            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(conStr);
                connection.Open();
                using (SqlCommand checkCommand = new SqlCommand("pro_bs_getUuidByUserName", connection))
                {
                    checkCommand.CommandType = CommandType.StoredProcedure;
                    checkCommand.Parameters.AddWithValue("@UserName", currentUserName);

                    var result = await checkCommand.ExecuteScalarAsync();
                    if (result == null || result.ToString() != sessionID)
                    {
                        return StatusCode(HttpStatusCode.Unauthorized);
                    }
                }

                var users = ConnectToDatabase();

                // 模糊搜索
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    users = users.Where(u => u.UserName.ToLower().Contains(searchTerm.ToLower())).ToList();
                }

                // 排序-帳號創立日期
                users = users.OrderByDescending(u => u.CreateTime).ToList();

                // 計算資料總數
                int totalRecords = users.Count();

                // 分頁
                users = users.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();


                var lowercaseUsers = users.Select(u => new
                {
                    id = u.Id,
                    userName = u.UserName.ToLower(),
                    password = u.Password.ToLower(),
                    loginTime = u.LoginTime?.ToString("yyyy-MM-dd"),
                    createTime = u.CreateTime?.ToString("yyyy-MM-dd"),
                    permission = u.Permission
                });

                // 判斷是否還有更多資料
                bool hasMore = (pageNumber * pageSize) < totalRecords;

                return Ok(new { code = 0, message = "請求成功", data = lowercaseUsers, hasMore = hasMore });
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

        // HTTP POST 用戶登入 API
        [HttpPost, Route("api/user/login")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Login(UserLoginModel model)

        {
            string pattern = @"^[a-zA-Z0-9_-]{4,16}$";
            if (
                !System.Text.RegularExpressions.Regex.IsMatch(model.UserName, pattern) ||
                !System.Text.RegularExpressions.Regex.IsMatch(model.Password, pattern))
            {
                return Ok(new { code = 1, message = "帳號或密碼格式錯誤，必須是4-16個字符，只能包含字母、數字、下劃線和連字符" });
            }

            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(conStr);
                connection.Open();

                using (SqlCommand command = new SqlCommand("pro_bs_getUserLogin", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserName", model.UserName);
                    command.Parameters.AddWithValue("@Password", model.Password);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        var response = HttpContext.Current.Response;
                        if (dataTable.Rows.Count > 0)
                        {
                            string userId = Convert.ToString(dataTable.Rows[0]["f_id"]);
                            string permission = Convert.ToString(dataTable.Rows[0]["f_permission"]);
                            DateTime? previousLoginTime = dataTable.Rows[0]["f_loginTime"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dataTable.Rows[0]["f_loginTime"]) : null;
                            // 更新新的登入狀態、登入時間和 UUID
                            using (SqlCommand updateCommand = new SqlCommand("pro_bs_updateAccountLoginInfo", connection))
                            {
                                updateCommand.CommandType = CommandType.StoredProcedure;
                                updateCommand.Parameters.AddWithValue("@UserId", userId);
                                updateCommand.Parameters.AddWithValue("@LoginTime", DateTime.Now);
                                updateCommand.Parameters.AddWithValue("@UUID", HttpContext.Current.Session.SessionID);
                                updateCommand.ExecuteNonQuery();
                            }
                            HttpContext.Current.Session["currentUser"] = model.UserName;
                            HttpCookie cookie1 = new HttpCookie("uuid");
                            HttpCookie cookie2 = new HttpCookie("permission");
                            cookie1.Value = HttpContext.Current.Session.SessionID;
                            cookie2.Value = permission;
                            response.Cookies.Add(cookie1);
                            response.Cookies.Add(cookie2);
                            return Ok(new { code = 0, message = "登入成功" });
                        }
                        else
                        {
                            return Ok(new { code = 1, message = "無效的帳號或密碼，請重新登入" });
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

        // HTTP POST 新增用戶 API
        [HttpPost, Route("api/user/add")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> CreateUser(UserAddModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string pattern = @"^[a-zA-Z0-9_-]{4,16}$";
            if (
                !System.Text.RegularExpressions.Regex.IsMatch(model.UserName, pattern) ||
                !System.Text.RegularExpressions.Regex.IsMatch(model.Password, pattern))
            {
                return Ok(new { code = 1, message = "帳號或密碼格式錯誤，必須是4-16個字符，只能包含字母、數字、下劃線和連字符222" });
            }

            if (
                String.IsNullOrEmpty(model.Permission))
            {
                return Ok(new { code = 1, message = "未選擇用戶權限" });
            }

            string sessionID = HttpContext.Current.Session.SessionID;
            string currentUserName = HttpContext.Current.Session["currentUser"] as string;

            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(conStr);
                connection.Open();

                // 驗證 sessionID 是否與資料庫中的 f_uuid 匹配
                using (SqlCommand checkCommand = new SqlCommand("pro_bs_getUuidByUserName", connection))
                {
                    checkCommand.CommandType = CommandType.StoredProcedure;
                    checkCommand.Parameters.AddWithValue("@UserName", currentUserName);

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
                    command.Parameters.AddWithValue("@UserName", model.UserName);
                    command.Parameters.AddWithValue("@Password", model.Password);
                    command.Parameters.AddWithValue("@CreateTime", DateTime.Now);
                    command.Parameters.AddWithValue("@Permission", model.Permission);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            string message = reader.GetString(0);
                            if (message == "User created successfully")
                            {
                                return Ok(new { code = 0, message = "創建成功" });
                            }
                            else
                            {
                                return Ok(new { code = 1, message = "用戶已存在" });
                            }
                        }
                        else
                        {
                            return InternalServerError(new Exception("No message returned from stored procedure."));
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

        // HTTP POST 刪除用戶 API
        [HttpPost, Route("api/user/delete")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> DeleteUser(UserDeleteModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string sessionID = HttpContext.Current.Session.SessionID;
            string currentUserName = HttpContext.Current.Session["currentUser"] as string;

            SqlConnection connection = null;

            try
            {

                connection = new SqlConnection(conStr);
                connection.Open();
                // 驗證 sessionID 是否與資料庫中的 f_uuid 匹配
                using (SqlCommand checkCommand = new SqlCommand("pro_bs_getUuidByUserName", connection))
                {
                    checkCommand.CommandType = CommandType.StoredProcedure;
                    checkCommand.Parameters.AddWithValue("@UserName", currentUserName);

                    var result = await checkCommand.ExecuteScalarAsync();
                    if (result == null || result.ToString() != sessionID)
                    {
                        return StatusCode(HttpStatusCode.Unauthorized);
                    }
                }

                // 刪除用戶
                using (SqlCommand command = new SqlCommand("pro_bs_deleteUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserName", model.UserName);
                    command.Parameters.AddWithValue("@CurrentUserName", currentUserName);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            string message = reader.GetString(0);
                            if (message == "User deleted successfully")
                            {
                                return Ok(new { code = 0, message = "刪除成功" });
                            }
                            else if (message == "Cannot delete yourself")
                            {
                                return Ok(new { code = 1, message = "不能刪除自己" });
                            }
                            else
                            {
                                return Ok(new { code = 2, message = "用戶不存在" });
                            }
                        }
                        else
                        {
                            return InternalServerError(new Exception("No message returned from stored procedure."));
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

        // HTTP POST 更新用戶 API
        [HttpPost, Route("api/user/update")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> EditUser(UserUpdateModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string pattern = @"^[a-zA-Z0-9_-]{4,16}$";
            if (
                !System.Text.RegularExpressions.Regex.IsMatch(model.UserName, pattern) ||
                !System.Text.RegularExpressions.Regex.IsMatch(model.Password, pattern))
            {
                return Ok(new { code = 1, message = "帳號或密碼格式錯誤，必須是4-16個字符，只能包含字母、數字、下劃線和連字符222" });
            }

            if (
                String.IsNullOrEmpty(model.Permission))
            {
                return Ok(new { code = 1, message = "未選擇用戶權限" });
            }

            string sessionID = HttpContext.Current.Session.SessionID;
            string currentUserName = HttpContext.Current.Session["currentUser"] as string;

            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(conStr);
                connection.Open();

                // 驗證 sessionID 是否與資料庫中的 f_uuid 匹配
                using (SqlCommand checkCommand = new SqlCommand("pro_bs_getUuidByUserName", connection))
                {
                    checkCommand.CommandType = CommandType.StoredProcedure;
                    checkCommand.Parameters.AddWithValue("@UserName", currentUserName);

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
                    command.Parameters.AddWithValue("@UserName", model.UserName);
                    command.Parameters.AddWithValue("@NewPassword", model.Password);
                    command.Parameters.AddWithValue("@NewPermission", model.Permission);
                    command.Parameters.AddWithValue("@CurrentUserName", currentUserName);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            string message = reader.GetString(0);
                            if (message == "User updated successfully")
                            {
                                return Ok(new { code = 0, message = "更新成功" });
                            }
                            else if (message == "Cannot edit your own account")
                            {
                                return Ok(new { code = 1, message = "不能修改自己的帳號" });
                            }
                            else if (message == "User not found")
                            {
                                return Ok(new { code = 2, message = "用戶不存在" });
                            }
                            else
                            {
                                return Ok(new { code = 3, message = "未知錯誤" });
                            }
                        }
                        else
                        {
                            return InternalServerError(new Exception("No message returned from stored procedure."));
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
}