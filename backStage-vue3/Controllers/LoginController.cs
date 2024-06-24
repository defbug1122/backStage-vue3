using System;
using System.Web.Http;
using backStage_vue3.Models;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System.Web;
using backStage_vue3.Utilities;

namespace backStage_vue3.Controllers
{
    public class LoginController : ApiController
    {
        // GET: Login
        /// <summary>
        /// HTTP POST 用户登入 API
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/user/login")]
        public async Task<IHttpActionResult> Login(UserLoginModel model)
        {

            string pattern = @"^[a-zA-Z0-9_-]{4,16}$";

            if (!System.Text.RegularExpressions.Regex.IsMatch(model.Un, pattern) ||
                !System.Text.RegularExpressions.Regex.IsMatch(model.Pwd, pattern))
            {
                return Ok(new { code = StatusResCode.InvalidFormat });
            }

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
                command.Parameters.AddWithValue("@un", model.Un);
                command.Parameters.AddWithValue("@pwd", hashPwd);
                command.Parameters.AddWithValue("@uuid", HttpContext.Current.Session.SessionID);
                SqlParameter statusCodeParam = new SqlParameter("@statusCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(statusCodeParam);

                await command.ExecuteNonQueryAsync();

                int statusCode = (int)statusCodeParam.Value;

                if (statusCode == 0)
                {
                    reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        string userId = Convert.ToString(reader["f_id"]);
                        string permission = Convert.ToString(reader["f_permission"]);

                        UserSessionModel sessionInfo = new UserSessionModel
                        {
                            CurrentUser = model.Un,
                            CurrentPermission = permission,
                            CurrentsessionID = HttpContext.Current.Session.SessionID,
                        };

                        HttpContext.Current.Session["userSessionInfo"] = sessionInfo;

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
                if (reader != null)
                {
                    reader.Close();
                }
            }

        }
    }
}