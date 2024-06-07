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
                    f_id = Convert.ToInt32(row["f_id"]),
                    f_userName = row["f_userName"].ToString(),
                    f_password = row["f_password"].ToString(),
                    f_createTime = Convert.ToDateTime(row["f_createTime"])
                };
                users.Add(user);
            }

            return users;
        }

        //public IEnumerable<User> GetAllUsers()
        //{

        //    var users = ConnectToDatabase();
        //    return users;
        //}

        [HttpGet, Route("api/Users")]
        [AllowAnonymous]
        [ResponseType(typeof(IEnumerable<UserModel>))]
        public async Task<IHttpActionResult> GetUsers()
        {
            try
            {
                var users = ConnectToDatabase();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // HTTP POST 方法定义用户登录 API
        [HttpPost, Route("api/Login")]
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

                            if (dataTable.Rows.Count > 0)
                            {
                                return Ok(new { message = "登入成功" });
                            }
                            else
                            {
                                return BadRequest("無效的帳號或密碼");
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

        public IHttpActionResult GetUser(int id)
        {
            var users = ConnectToDatabase();
            var user = users.FirstOrDefault((p) => p.f_id == id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
    }
}