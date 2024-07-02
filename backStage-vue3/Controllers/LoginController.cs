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

            if (!System.Text.RegularExpressions.Regex.IsMatch(model.Un, pattern) ||
                !System.Text.RegularExpressions.Regex.IsMatch(model.Pwd, pattern))
            {
                result.Code = (int)StatusResCode.InvalidFormat;
                return Ok(result);
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
                string Id = HttpContext.Current.Session.SessionID;

                if (statusCode == 0)
                {
                    reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        string userId = Convert.ToString(reader["f_id"]);
                        string permission = reader["f_permission"].ToString();

                        UserSessionModel sessionInfo = new UserSessionModel
                        {
                            Un = model.Un,
                            Permission = Convert.ToInt32(reader["f_permission"]),
                            SessionID = HttpContext.Current.Session.SessionID,
                        };

                        HttpContext.Current.Session["userSessionInfo"] = sessionInfo;

                        HttpCookie uuidCookie = new HttpCookie("uuid", HttpContext.Current.Session.SessionID);
                        HttpCookie permissionCookie = new HttpCookie("permission", permission);
                        HttpCookie currentUserCookie = new HttpCookie("currentUser", model.Un);

                        HttpContext.Current.Response.Cookies.Add(uuidCookie);
                        HttpContext.Current.Response.Cookies.Add(permissionCookie);
                        HttpContext.Current.Response.Cookies.Add(currentUserCookie);

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