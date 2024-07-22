using System;
using System.Web.Http;
using backStage_vue3.Models;
using System.Data.SqlClient;
using System.Data;
using System.Net;
using System.Threading.Tasks;
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
        public async Task<IHttpActionResult> GetMembers([FromUri] GetMemberRequest model)
        {
            var result = new GetMemberResponseDto();

            if (model == null)
            {
                result.Code = (int)StatusResCode.MissingParams;
                return Ok(result);
            }

            // 關鍵字搜尋不能超過16個字、頁數必須大於1、筆數只能查詢剛好10筆
            if ((model.SearchTerm != null && model.SearchTerm.Length > 16) || model.PageNumber <= 0 || model.PageSize != 10)
            {
                result.Code = (int)StatusResCode.InvalidFormat;
                return Ok(result);
            }

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
                command.Parameters.AddWithValue("@searchTerm", model.SearchTerm);
                command.Parameters.AddWithValue("@pageNumber", model.PageNumber);
                command.Parameters.AddWithValue("@pageSize", model.PageSize);
                command.Parameters.AddWithValue("@sortBy", model.SortBy);
                SqlParameter statusCodeParam = new SqlParameter("@statusCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(statusCodeParam);
                await command.ExecuteNonQueryAsync();
                int statusCode = (int)statusCodeParam.Value;

                if (statusCode == (int)StatusResCode.UnMatchSessionId)
                {
                    return StatusCode(HttpStatusCode.Unauthorized);
                }

                reader = await command.ExecuteReaderAsync();
                List<MemberModel> members = new List<MemberModel>();
                int totalRecords = 0;

                while (await reader.ReadAsync())
                {
                    MemberModel member = new MemberModel
                    {
                        MemberId = Convert.ToInt32(reader["f_memberId"]),
                        MemberName = reader["f_memberName"].ToString(),
                        Level = Convert.ToByte(reader["f_level"]),
                        TotalSpent = Convert.ToDecimal(reader["f_totalSpent"]),
                        Status = Convert.ToBoolean(reader["f_status"]),
                    };
                    members.Add(member);
                    totalRecords = Convert.ToInt32(reader["TotalRecords"]);
                }

                bool hasMore = (model.PageNumber * model.PageSize) < totalRecords;
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

            if (model == null)
            {
                result.Code = (int)StatusResCode.MissingParams;
                return Ok(result);
            }

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
                command.Parameters.AddWithValue("@memberId",model.MemberId);
                command.Parameters.AddWithValue("@status", model.Status);
                SqlParameter statusCodeParam = new SqlParameter("@statusCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(statusCodeParam);
                await command.ExecuteNonQueryAsync();
                int statusCode = (int)statusCodeParam.Value;

                if (statusCode == (int)StatusResCode.Success)
                {
                    result.Code = (int)StatusResCode.Success;
                    return Ok(result);
                }
                else if (statusCode == (int)StatusResCode.UnMatchSessionId)
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

            if (model == null)
            {
                result.Code = (int)StatusResCode.MissingParams;
                return Ok(result);
            }

            // 目前只有4個會員等級狀態
            if (model.Level > 4)
            {
                result.Code = (int)StatusResCode.InvalidFormat;
                return Ok(result);
            }

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
                command.Parameters.AddWithValue("@memberId", model.MemberId);
                command.Parameters.AddWithValue("@level", model.Level);
                SqlParameter statusCodeParam = new SqlParameter("@statusCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(statusCodeParam);
                await command.ExecuteNonQueryAsync();
                int statusCode = (int)statusCodeParam.Value;

                if (statusCode == (int)StatusResCode.Success)
                {
                    result.Code = (int)StatusResCode.Success;
                    return Ok(result);
                }
                else if (statusCode == (int)StatusResCode.UnMatchSessionId)
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