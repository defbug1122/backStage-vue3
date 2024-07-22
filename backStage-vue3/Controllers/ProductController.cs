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
using backStage_vue3.Utilities;
using System.Configuration;

namespace backStage_vue3.Controllers
{
    public class ProductController : BaseController
    {
        // 限制圖片最大傳輸大小
        private int maxFileSizeInBytes = 2 * 1024 * 1024;

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

                // 從Web.config 抓取安全庫存量
                int safetyStock = int.Parse(ConfigurationManager.AppSettings["SafetyStock"]);

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
                    };
                    products.Add(product);
                    totalRecords = Convert.ToInt32(reader["TotalRecords"]);
                }

                bool hasMore = (model.PageNumber * model.PageSize) < totalRecords;
                result.Code = (int)StatusResCode.Success;
                result.Data = products;
                result.HasMore = hasMore;
                result.SafetyStock = safetyStock;
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

            // 商品名稱不可為空、描述不可超過50個字、價格不能為負值、庫存量不能為負值、種類不能為負值且不能超過3
            if ((model.Name.Length == 0 || model.Name.Length > 20) || model.Describe.Length > 50 || model.Price < 0 || model.Stock < 0 || (model.Type > 3 || model.Type < 0))
            {
                result.Code = (int)StatusResCode.InvalidFormat;
                return Ok(result);
            }

            // 缺少商品封面圖片
            if (model.ImagePath1 == null)
            {
                result.Code = (int)StatusResCode.MissingCoverImage;
                return Ok(result);
            }

            string[] imagePaths;

            try
            {
                imagePaths = ProcessBase64Images(new string[] { model.ImagePath1, model.ImagePath2, model.ImagePath3 }, maxFileSizeInBytes);
            }
            catch (ArgumentException ex)
            {
                if (ex.Message == "InvalidFormat" || ex.Message == "InvalidBase64FormatError")
                {
                    result.Code = (int)StatusResCode.InvalidFormat;
                }
                else if (ex.Message == "ImageFormatError")
                {
                    result.Code = (int)StatusResCode.ImageFormatError;
                }
                else if (ex.Message == "ImageFileIsLarge")
                {
                    result.Code = (int)StatusResCode.ImageFileIsLarge;
                }
                else
                {
                    result.Code = (int)StatusResCode.Failed;
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
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
                command.Parameters.AddWithValue("@productName", model.Name.Trim());
                command.Parameters.AddWithValue("@imagePath1", (object)imagePaths[0] ?? DBNull.Value);
                command.Parameters.AddWithValue("@imagePath2", (object)imagePaths[1] ?? DBNull.Value);
                command.Parameters.AddWithValue("@imagePath3", (object)imagePaths[2] ?? DBNull.Value);
                command.Parameters.AddWithValue("@productPrice", model.Price);
                command.Parameters.AddWithValue("@productType", model.Type);
                command.Parameters.AddWithValue("@productDescribe", model.Describe.Trim());
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
                    // 刪除已上傳的圖片
                    DeleteFiles(imagePaths);
                    result.Code = (int)StatusResCode.Failed;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                // 刪除已上傳的圖片
                DeleteFiles(imagePaths);
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

            // 商品名稱不可為空，超過20個字、描述不可超過50個字、價格不能為負值、庫存量不能為負值、種類不能為負值且不能超過3
            if ((model.Name.Length == 0 || model.Name.Length > 20) || model.Describe.Length > 50 || model.Price <= 0 || model.Stock < 0 || (model.Type > 3 || model.Type < 0))
            {
                result.Code = (int)StatusResCode.InvalidFormat;
                return Ok(result);
            }

            // 缺少商品封面圖片
            if (model.ImagePath1 == null)
            {
                result.Code = (int)StatusResCode.MissingCoverImage;
                return Ok(result);
            }

            string[] imagePaths;

            try
            {
                imagePaths = ProcessBase64Images(new string[] { model.ImagePath1, model.ImagePath2, model.ImagePath3 }, maxFileSizeInBytes);
            }
            catch (ArgumentException ex)
            {
                if (ex.Message == "InvalidFormat" || ex.Message == "InvalidBase64FormatError")
                {
                    result.Code = (int)StatusResCode.InvalidFormat;
                }
                else if (ex.Message == "ImageFormatError")
                {
                    result.Code = (int)StatusResCode.ImageFormatError;
                }
                else if (ex.Message == "ImageFileIsLarge")
                {
                    result.Code = (int)StatusResCode.ImageFileIsLarge;
                }
                else
                {
                    result.Code = (int)StatusResCode.Failed;
                }
                return Ok(result);
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
                command.Parameters.AddWithValue("@productName", model.Name.Trim());
                command.Parameters.AddWithValue("@imagePath1", (object)imagePaths[0] ?? DBNull.Value);
                command.Parameters.AddWithValue("@imagePath2", (object)imagePaths[1] ?? DBNull.Value);
                command.Parameters.AddWithValue("@imagePath3", (object)imagePaths[2] ?? DBNull.Value);
                command.Parameters.AddWithValue("@productPrice", model.Price);
                command.Parameters.AddWithValue("@productType", model.Type);
                command.Parameters.AddWithValue("@productDescribe", model.Describe.Trim());
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
                    // 刪除上上傳圖片
                    DeleteFiles(imagePaths);
                    result.Code = (int)StatusResCode.Failed;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                // 刪除上上傳圖片
                DeleteFiles(imagePaths);
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

                SqlParameter imagePath1Param = new SqlParameter("@imagePath1", SqlDbType.VarChar, 24)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(imagePath1Param);

                SqlParameter imagePath2Param = new SqlParameter("@imagePath2", SqlDbType.VarChar, 24)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(imagePath2Param);

                SqlParameter imagePath3Param = new SqlParameter("@imagePath3", SqlDbType.VarChar, 24)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(imagePath3Param);

                await command.ExecuteNonQueryAsync();

                int statusCode = (int)statusCodeParam.Value;

                if (statusCode == (int)StatusResCode.Success)
                {
                    string[] imagePaths = {
                        imagePath1Param.Value.ToString(),
                        imagePath2Param.Value.ToString(),
                        imagePath3Param.Value.ToString()
                    };

                    // 刪除圖片文件
                    DeleteFiles(imagePaths);
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
        /// 處理商品Base64圖片
        /// </summary>
        /// <param name="base64Images"></param>
        /// <returns></returns>
        private string[] ProcessBase64Images(string[] base64Images, int maxFileSizeInBytes)
        {
            string[] imagePaths = new string[base64Images.Length];

            for (int i = 0; i < base64Images.Length; i++)
            {
                var base64Image = base64Images[i];

                // 空字串代表不更新資料庫圖片
                if (base64Image == "")
                {
                    imagePaths[i] = "";
                }
                else if (!string.IsNullOrEmpty(base64Image))
                {
                    // 分為檔案格式、base64碼
                    var imageParts = base64Image.Split(',');

                    // 驗證圖片格式、檔案大小
                    ImageValidator.ValidateBase64Image(imageParts, maxFileSizeInBytes);

                    // 檔案格式
                    var header = imageParts[0];

                    // base64碼
                    var data = imageParts[1];
                    var fileType = header.Split(';')[0].Split('/')[1];

                    // 將base64碼 轉換成圖像
                    var bytes = Convert.FromBase64String(data);
                    var fileExtension = fileType;
                    var newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + (i + 1) + "." + fileExtension;
                    var uploadsDir = HttpContext.Current.Server.MapPath("~/Uploads");

                    // 確保目錄存在
                    if (!Directory.Exists(uploadsDir))
                    {
                        Directory.CreateDirectory(uploadsDir);
                    }

                    var filePath = Path.Combine(uploadsDir, newFileName);

                    // 將圖檔寫入資料夾
                    File.WriteAllBytes(filePath, bytes);
                    imagePaths[i] = $"{newFileName}";
                }
            }

            return imagePaths;
        }


        /// <summary>
        /// 刪除圖片
        /// </summary>
        /// <param name="filePaths"></param>
        private static void DeleteFiles(string[] filePaths)
        {
            foreach (var path in filePaths)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    var filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads"), path);

                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
            }
        }
    }
}