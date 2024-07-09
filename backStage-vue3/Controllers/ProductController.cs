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
    public class ProductController : BaseController
    {
        /// <summary>
        /// HTTP GET 取得商品列表 API
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet, Route("api/product/list")]
        public async Task<IHttpActionResult> GetMembers([FromUri] ProductModel model)
        {

            var result = new GetProductResponseDto();

            if (model == null)
            {
                result.Code = (int)StatusResCode.MissingParams;
                return Ok(result);
            }

            if ((model.SearchTerm != null && model.SearchTerm.Length > 16) || model.PageNumber <= 0 || model.PageSize != 10)
            {
                result.Code = (int)StatusResCode.InvalidFormat;
                return Ok(result);
            }

            bool checkPermission = (UserSession.Permission & (int)Permissions.ViewProduct) == (int)Permissions.ViewProduct;

            if (!checkPermission)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            if (model.SearchTerm != null && model.SearchTerm.Length > 16)
            {
                result.Code = (int)StatusResCode.InvalidFormat;
                return Ok(result);
            }


            SqlConnection connection = null;
            SqlCommand command = null;
            SqlDataReader reader = null;

            try
            {
                connection = new SqlConnection(SqlConfig.conStr);
                await connection.OpenAsync();
                command = new SqlCommand("pro_bs_getAllProducts", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@currentUserId", UserSession.Id);
                command.Parameters.AddWithValue("@currentSessionId", UserSession.SessionID);
                command.Parameters.AddWithValue("@currentPermission", UserSession.Permission);
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


                if (statusCode == 5)
                {
                    return StatusCode(HttpStatusCode.Unauthorized);
                }

                reader = await command.ExecuteReaderAsync();

                List<ProductModel> products = new List<ProductModel>();

                int totalRecords = 0;

                while (await reader.ReadAsync())
                {
                    ProductModel product = new ProductModel
                    {
                        ProductId = Convert.ToInt32(reader["f_productId"]),
                        Name = reader["f_name"].ToString(),
                        ImagePath = reader["f_imagePath"].ToString(),
                        Type = Convert.ToByte(reader["f_type"]),
                        Price = Convert.ToDecimal(reader["f_price"]),
                        Active = Convert.ToBoolean(reader["f_active"]),
                        Describe = reader["f_describe"].ToString(),
                        Stock = Convert.ToInt32(reader["f_stock"]),
                    };
                    products.Add(product);
                    totalRecords = Convert.ToInt32(reader["TotalRecords"]);
                }

                bool hasMore = (model.PageNumber * model.PageSize) < totalRecords;

                result.Code = (int)StatusResCode.Success;
                result.Data = products;
                result.HasMore = hasMore;

                return Ok(result);
            }
            catch (Exception ex)
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
        /// HTTP POST 新增商品 API
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/product/add")]
        public async Task<IHttpActionResult> CreateProduct(ProductAddModel model)
        {
            var result = new AddUserResponseDto();
            bool checkPermission = (UserSession.Permission & (int)Permissions.AddProduct) == (int)Permissions.AddProduct;

            if (!checkPermission)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            if (model == null)
            {
                result.Code = (int)StatusResCode.MissingParams;
                return Ok(result);
            }

            if (model.Name.Length > 20 || model.Describe.Length > 50 || model.Price < 0 || model.Stock < 0 || (model.Type > 3 || model.Type < 0))
            {
                result.Code = (int)StatusResCode.InvalidFormat;
                return Ok(result);
            }

            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                connection = new SqlConnection(SqlConfig.conStr);
                connection.Open();
                command = new SqlCommand("pro_bs_addNewProduct", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@productName", model.Name);
                command.Parameters.AddWithValue("@imagePath", model.ImageFile);
                command.Parameters.AddWithValue("@productPrice", model.Price);
                command.Parameters.AddWithValue("@productType", model.Type);
                command.Parameters.AddWithValue("@productDescribe", model.Describe);
                command.Parameters.AddWithValue("@productStock", model.Stock);
                command.Parameters.AddWithValue("@productActive", model.Active);

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
                    command.Parameters.Clear();
                }
            }

        }
    }
}