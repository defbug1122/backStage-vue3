using System;
using System.Web.Http;
using backStage_vue3.Models;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System.Web;
using backStage_vue3.Utilities;
using backStage_vue3.Services;

namespace backStage_vue3.Controllers
{
    public class LoginController : ApiController
    {
        /// <summary>
        /// HTTP POST 用户登入 API
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/user/login")]
        public async Task<IHttpActionResult> Login(UserLoginModel model)
        {
            var result = new UserLoginResponseDto();
            string pattern = @"^[a-zA-Z0-9_-]{4,16}$";

            if (model.Pwd == null || model.UserName == null) {
                result.Code = (int)StatusResCode.MissingParams;
                return Ok(result);
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(model.UserName, pattern) ||
                !System.Text.RegularExpressions.Regex.IsMatch(model.Pwd, pattern))
            {
                result.Code = (int)StatusResCode.InvalidFormat;
                return Ok(result);
            }

            // 密碼使用 SHA256 加密
            string hashPwd = HashHelper.ComputeSha256Hash(model.Pwd);
            SqlConnection connection = null;
            SqlCommand command = null;
            SqlDataReader reader = null;

            try
            {
                connection = new SqlConnection(SqlConfig.conStr);
                await connection.OpenAsync();
                command = new SqlCommand("pro_bs_getUserLogin", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@userName", model.UserName);
                command.Parameters.AddWithValue("@pwd", hashPwd);
                command.Parameters.AddWithValue("@sessionId", HttpContext.Current.Session.SessionID);
                SqlParameter statusCodeParam = new SqlParameter("@statusCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(statusCodeParam);
                await command.ExecuteNonQueryAsync();
                int statusCode = (int)statusCodeParam.Value;
                string Id = HttpContext.Current.Session.SessionID;

                if (statusCode == (int)StatusResCode.Success)
                {
                    reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        string userId = Convert.ToString(reader["f_userId"]);
                        string permission = reader["f_permission"].ToString();

                        UserSessionModel sessionInfo = new UserSessionModel
                        {
                            UserName = model.UserName,
                            Id = Convert.ToInt32(reader["f_userId"]),
                            Permission = Convert.ToInt32(reader["f_permission"]),
                            SessionID = HttpContext.Current.Session.SessionID,
                        };

                        HttpContext.Current.Session["userSessionInfo"] = sessionInfo;
                        HttpCookie uuidCookie = new HttpCookie("uuid", HttpContext.Current.Session.SessionID);
                        HttpCookie permissionCookie = new HttpCookie("permission", permission);
                        HttpCookie currentUserCookie = new HttpCookie("currentUser", model.UserName);
                        HttpCookie userIdCookie = new HttpCookie("userId", userId);
                        HttpContext.Current.Response.Cookies.Add(uuidCookie);
                        HttpContext.Current.Response.Cookies.Add(permissionCookie);
                        HttpContext.Current.Response.Cookies.Add(currentUserCookie);
                        HttpContext.Current.Response.Cookies.Add(userIdCookie);
                        var sessionService = new UserSessionCacheService();
                        sessionService.AddUserSession(sessionInfo);
                        LoggerHelper.Log(LogLevel.Info, $"session-----${HttpContext.Current.Session.Timeout}");
                        result.Code = (int)StatusResCode.Success;
                        return Ok(result);
                    }
                    else
                    {
                        result.Code = (int)StatusResCode.Failed;
                        return Ok(result);
                    }
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

                if (reader != null)
                {
                    reader.Close();
                }
            }
        }
    }
}