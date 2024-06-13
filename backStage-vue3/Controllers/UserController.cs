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
                string query = "SELECT * FROM t_member";

                using (SqlCommand command = new SqlCommand(query, connection))
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
                    LoginStatus = row["f_loginStatus"] != DBNull.Value ? Convert.ToBoolean(row["f_loginStatus"]) : false,
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
            try
            {
                var users = ConnectToDatabase();

                // 进行模糊搜索
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    users = users.Where(u => u.UserName.ToLower().Contains(searchTerm.ToLower())).ToList();
                }

                // 排序
                users = users.OrderByDescending(u => u.CreateTime).ToList();

                // 分页
                users = users.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                var lowercaseUsers = users.Select(u => new
                {
                    id = u.Id,
                    userName = u.UserName.ToLower(),
                    password = u.Password.ToLower(),
                    loginTime = u.LoginTime,
                    createTime = u.CreateTime,
                    loginStatus = u.LoginStatus,
                    permission = u.Permission
                });
                return Ok(new { code = 0, message = "請求成功", data = lowercaseUsers });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // HTTP POST 用戶登入 API
        [HttpPost, Route("api/user/login")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Login(UserLoginModel model)

        {
            try
            {
                using (SqlConnection connection = new SqlConnection(conStr))
                {
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
                            //var token = GenerateToken(model.UserName);

                            var response = HttpContext.Current.Response;
                            if (dataTable.Rows.Count > 0)
                            {
                                string userId = Convert.ToString(dataTable.Rows[0]["f_id"]);
                                string permission = Convert.ToString(dataTable.Rows[0]["f_permission"]);
                                bool isAlreadyLoggedIn = dataTable.Rows[0]["f_loginStatus"] != DBNull.Value && Convert.ToBoolean(dataTable.Rows[0]["f_loginStatus"]);
                                DateTime? previousLoginTime = dataTable.Rows[0]["f_loginTime"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dataTable.Rows[0]["f_loginTime"]) : null;
                                // 更新新的登入狀態和時間
                                using (SqlCommand updateCommand = new SqlCommand("UPDATE t_member SET f_loginStatus = 1, f_loginTime = @LoginTime WHERE f_id = @UserId", connection))
                                {
                                    updateCommand.Parameters.AddWithValue("@UserId", userId);
                                    updateCommand.Parameters.AddWithValue("@LoginTime", DateTime.Now);
                                    updateCommand.ExecuteNonQuery();
                                }
                                HttpCookie cookie1 = new HttpCookie("uuid");
                                HttpCookie cookie2 = new HttpCookie("permission");
                                cookie1.Value = HttpContext.Current.Session.SessionID;
                                cookie2.Value = permission;
                                response.Cookies.Add(cookie1);
                                response.Cookies.Add(cookie2);
                                return Ok(new { code = 0,message = "登入成功" });
                            }
                            else
                            {
                                return Ok(new { code = 1, message = "無效的帳號或密碼，請重新登入" });
                            }
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
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

            try
            {
                using (SqlConnection connection = new SqlConnection(conStr))
                {
                    //await connection.OpenAsync();
                    //using (SqlCommand command = new SqlCommand("SELECT f_loginStatus FROM t_member WHERE f_userName = @UserName", connection))
                    //{
                    //    command.Parameters.AddWithValue("@UserName", model.UserName);

                    //    var loginStatus = (bool?)await command.ExecuteScalarAsync();
                    //    if (loginStatus == true)
                    //    {
                    //        // 如果已經有同樣的帳號登入，則踢出前者
                    //        using (SqlCommand updateCommand = new SqlCommand("UPDATE t_member SET f_loginStatus = 0 WHERE f_userName = @UserName AND f_loginStatus = 1", connection))
                    //        {
                    //            updateCommand.Parameters.AddWithValue("@UserName", model.UserName);
                    //            await updateCommand.ExecuteNonQueryAsync();
                    //        }

                    //        return StatusCode(HttpStatusCode.Unauthorized); // 401 狀態碼
                    //    }
                    //}

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
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public IHttpActionResult GetUser(string username)
        {
            var users = ConnectToDatabase();
            var user = users.FirstOrDefault((p) => p.UserName == username);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
    }
}