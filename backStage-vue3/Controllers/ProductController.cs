using System;
using System.Web.Http;
using backStage_vue3.Models;
using System.Data.SqlClient;
using System.Data;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Collections.Generic;
using System.IO;

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
        public async Task<IHttpActionResult> GetProducts([FromUri] GetProdctRequest model)
        {

            var result = new GetProductResponseDto();

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

            bool checkPermission = (UserSession.Permission & (int)Permissions.ViewProduct) == (int)Permissions.ViewProduct;

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
                command = new SqlCommand("pro_bs_getAllProducts", connection)
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

                List<ProductModel> products = new List<ProductModel>();

                int totalRecords = 0;
                int safetyStock = 5;

                while (await reader.ReadAsync())
                {
                    ProductModel product = new ProductModel
                    {
                        ProductId = Convert.ToInt32(reader["f_productId"]),
                        Name = reader["f_name"].ToString(),
                        ImagePath1 = reader["f_imagePath1"].ToString(),
                        ImagePath2 = reader["f_imagePath2"].ToString(),
                        ImagePath3 = reader["f_imagePath3"].ToString(),
                        Type = Convert.ToByte(reader["f_type"]),
                        Price = Convert.ToDecimal(reader["f_price"]),
                        Active = Convert.ToBoolean(reader["f_active"]),
                        Describe = reader["f_describe"].ToString(),
                        Stock = Convert.ToInt32(reader["f_stock"]),
                        StockWarning = Convert.ToInt32(reader["f_stock"]) < safetyStock ? true : false,
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

            // 名稱不可超過20個字、描述不可超過50個字、價格不能為負值、庫存量不能為負值、種類不能為負值且不能超過3
            if (model.Name.Length > 20 || model.Describe.Length > 50 || model.Price < 0 || model.Stock < 0 || (model.Type > 3 || model.Type < 0))
            {
                result.Code = (int)StatusResCode.InvalidFormat;
                return Ok(result);
            }

            string[] imagePaths = new string[3];

            for (int i = 0; i < 3; i++)
            {
                var base64Image = i == 0 ? model.ImagePath1 : i == 1 ? model.ImagePath2 : model.ImagePath3;

                if (!string.IsNullOrEmpty(base64Image))
                {
                    // 分割 Base64 字符
                    var imageParts = base64Image.Split(',');

                    if (imageParts.Length != 2)
                    {
                        result.Code = (int)StatusResCode.InvalidFormat;
                        return Ok(result);
                    }

                    // Base64 前半段
                    var header = imageParts[0];

                    // Base64 後半段數據
                    var data = imageParts[1];

                    // 獲取文件類型
                    var fileType = header.Split(';')[0].Split('/')[1];

                    if (fileType != "jpeg" && fileType != "png")
                    {
                        result.Code = (int)StatusResCode.ImageFormatError;
                        return Ok(result);
                    }

                    // 將base64 轉換為真實圖像
                    var bytes = Convert.FromBase64String(data);

                    if (bytes.Length > 2 * 1024 * 1024)
                    {
                        result.Code = (int)StatusResCode.ImageFileIsLarge;
                        return Ok(result);
                    }

                    var fileExtension = fileType;

                    // 檔名加上時間戳
                    var newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + (i + 1) + "." + fileExtension;
                    var filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads"), newFileName);

                    // 確保檔名目錄存在，如果不存在創建目錄
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    // 將圖檔寫入資料夾
                    File.WriteAllBytes(filePath, bytes);
                    imagePaths[i] = $"{newFileName}";
                }
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
                command.Parameters.AddWithValue("@imagePath1", (object)imagePaths[0] ?? DBNull.Value);
                command.Parameters.AddWithValue("@imagePath2", (object)imagePaths[1] ?? DBNull.Value);
                command.Parameters.AddWithValue("@imagePath3", (object)imagePaths[2] ?? DBNull.Value);
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
                    command.Parameters.Clear();
                }
            }

        }

        /// <summary>
        /// HTTP POST 編輯商品 API
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/product/edit")]
        public async Task<IHttpActionResult> UpdateProduct(UpdateProductModel model)
        {
            var result = new UpdateProductResponseDto();
            bool checkPermission = (UserSession.Permission & (int)Permissions.EditProduct) == (int)Permissions.EditProduct;

            if (!checkPermission)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            if (model == null)
            {
                result.Code = (int)StatusResCode.MissingParams;
                return Ok(result);
            }

            // 名稱不可超過20個字、描述不可超過50個字、價格不能為負值、庫存量不能為負值、種類不能為負值且不能超過3
            if (model.Name.Length > 20 || model.Describe.Length > 50 || model.Price < 0 || model.Stock < 0 || (model.Type > 3 || model.Type < 0))
            {
                result.Code = (int)StatusResCode.InvalidFormat;
                return Ok(result);
            }

            string[] imagePaths = new string[3];

            for (int i = 0; i < 3; i++)
            {
                var base64Image = i == 0 ? model.ImagePath1 : i == 1 ? model.ImagePath2 : model.ImagePath3;

                if (base64Image == "deleted")
                {
                    imagePaths[i] = "deleted"; // 标记为删除图片
                }

                else if (!string.IsNullOrEmpty(base64Image))
                {
                    // 分割 Base64 字符
                    var imageParts = base64Image.Split(',');

                    if (imageParts.Length != 2)
                    {
                        result.Code = (int)StatusResCode.InvalidFormat;
                        return Ok(result);
                    }

                    // Base64 前半段
                    var header = imageParts[0];

                    // Base64 後半段數據
                    var data = imageParts[1];

                    // 獲取文件類型
                    var fileType = header.Split(';')[0].Split('/')[1];

                    if (fileType != "jpeg" && fileType != "png")
                    {
                        result.Code = (int)StatusResCode.ImageFormatError;
                        return Ok(result);
                    }

                    // 將base64 轉換為真實圖像
                    var bytes = Convert.FromBase64String(data);

                    if (bytes.Length > 2 * 1024 * 1024)
                    {
                        result.Code = (int)StatusResCode.ImageFileIsLarge;
                        return Ok(result);
                    }

                    var fileExtension = fileType;

                    // 檔名加上時間戳
                    var newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + (i + 1) + "." + fileExtension;
                    var filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads"), newFileName);

                    // 確保檔名目錄存在，如果不存在創建目錄
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    // 將圖檔寫入資料夾
                    File.WriteAllBytes(filePath, bytes);
                    imagePaths[i] = $"{newFileName}";
                }
            }

            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                connection = new SqlConnection(SqlConfig.conStr);
                connection.Open();
                command = new SqlCommand("pro_bs_editProduct", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@productId", model.ProductId);
                command.Parameters.AddWithValue("@productName", model.Name);
                command.Parameters.AddWithValue("@imagePath1", (object)imagePaths[0] ?? DBNull.Value);
                command.Parameters.AddWithValue("@imagePath2", (object)imagePaths[1] ?? DBNull.Value);
                command.Parameters.AddWithValue("@imagePath3", (object)imagePaths[2] ?? DBNull.Value);
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
                    command.Parameters.Clear();
                }
            }

        }

        /// <summary>
        /// HTTP POST 刪除商品 API
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/product/delete")]
        public async Task<IHttpActionResult> DeleteProduct(DeleteProductModel model)
        {
            var result = new DeleteProductResponseDto();
            bool checkPermission = (UserSession.Permission & (int)Permissions.DeleteProduct) == (int)Permissions.DeleteProduct;

            if (!checkPermission)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            if (model == null)
            {
                result.Code = (int)StatusResCode.MissingParams;
                return Ok(result);
            }

            if (model.ProductId < 0)
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
                command = new SqlCommand("pro_bs_delProduct", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@productId", model.ProductId);

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
                    command.Parameters.Clear();
                }
            }

        }
    }
}