using System;
using System.Web.Http;
using backStage_vue3.Models;
using System.Data.SqlClient;
using System.Data;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Collections.Generic;

namespace backStage_vue3.Controllers
{
    public class MemberController : BaseController
    {
        /// <summary>
        /// HTTP GET 取得會員列表 API
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet, Route("api/member/list")]
        public async Task<IHttpActionResult> GetMembers(string searchTerm = "", int pageNumber = 1, int pageSize = 10, int sortBy = 1)
        {

            var result = new GetMemberResponseDto();

            bool checkPermission = (UserSession.Permission & (int)Permissions.ViewMember) == (int)Permissions.ViewMember;

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
                command = new SqlCommand("pro_bs_getAllMembers", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                //command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@currentUn", UserSession.Un);
                command.Parameters.AddWithValue("@currentSessionID", UserSession.SessionID);
                command.Parameters.AddWithValue("@searchTerm", searchTerm);
                command.Parameters.AddWithValue("@pageNumber", pageNumber);
                command.Parameters.AddWithValue("@pageSize", pageSize);
                command.Parameters.AddWithValue("@sortBy", sortBy);

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

                List<MemberModel> members = new List<MemberModel>();

                while (await reader.ReadAsync())
                {
                    MemberModel member = new MemberModel
                    {
                        Id = Convert.ToInt32(reader["f_mId"]),
                        Mn = reader["f_mn"].ToString(),
                        Level = Convert.ToByte(reader["f_level"]),
                        TotalSpent = Convert.ToDecimal(reader["f_totalSpent"]),
                        Status = Convert.ToBoolean(reader["f_status"]),
                    };
                    members.Add(member);
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
                result.Data = members;
                result.HasMore = hasMore;

                return Ok(result);
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
            finally
            {
                if (connection != null)
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
        /// HTTP POST 更新會員狀態 API
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/member/updateStatus")]
        public async Task<IHttpActionResult> UpdateMemberStatus(UpdateStatusModel model)
        {
            var result = new UpdateStatusResponseDto();

            bool checkPermission = (UserSession.Permission & (int)Permissions.SuspendMember) == (int)Permissions.SuspendMember;

            if (!checkPermission)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                connection = new SqlConnection(SqlConfig.conStr);
                connection.Open();
                command = new SqlCommand("pro_bs_editMemberStatus", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                //command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@currentUn", UserSession.Un);
                command.Parameters.AddWithValue("@currentSessionID", UserSession.SessionID);
                command.Parameters.AddWithValue("@mId",model.MemberId);
                command.Parameters.AddWithValue("@status", model.Status);

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
        /// HTTP POST 更新會員等級 API
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/member/updateLevel")]
        public async Task<IHttpActionResult> UpdateMemberLevel(UpdateLevelModel model)
        {
            var result = new UpdateLevelResponseDto();

            bool checkPermission = (UserSession.Permission & (int)Permissions.SetMemberLevel) == (int)Permissions.SetMemberLevel;

            if (!checkPermission)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                connection = new SqlConnection(SqlConfig.conStr);
                connection.Open();
                command = new SqlCommand("pro_bs_editMemberLevel", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                //command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@currentUn", UserSession.Un);
                command.Parameters.AddWithValue("@currentSessionID", UserSession.SessionID);
                command.Parameters.AddWithValue("@mId", model.MemberId);
                command.Parameters.AddWithValue("@level", model.Level);

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

    }
}