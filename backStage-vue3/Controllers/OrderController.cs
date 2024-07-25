using System;
using System.Web.Http;
using backStage_vue3.Models;
using System.Data.SqlClient;
using System.Data;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using backStage_vue3.Models.Order;

namespace backStage_vue3.Controllers
{
    public class OrderController : BaseController
    {
        /// <summary>
        /// HTTP GET 取得訂單列表 API
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet, Route("api/orders/list")]
        public async Task<IHttpActionResult> GetOrders([FromUri] GetOrderRequest model)
        {
            var result = new GetOrderResponseDto();

            if (model == null)
            {
                result.Code = (int)StatusResCode.MissingParams;
                return Ok(result);
            }

            // 關鍵字搜尋不能超過17個字、頁數必須大於1、筆數只能查詢剛好10筆
            if ((model.SearchTerm != null && model.SearchTerm.Length > 17) || model.PageNumber <= 0 || model.PageSize != 10 || (model.SortBy >3 || model.SortBy < 0))
            {
                result.Code = (int)StatusResCode.InvalidFormat;
                return Ok(result);
            }

            bool checkPermission = (UserSession.Permission & (int)Permissions.ViewOrder) == (int)Permissions.ViewOrder;

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
                command = new SqlCommand("pro_bs_getAllOrders", connection)
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
                reader = await command.ExecuteReaderAsync();
                List<OrderModel> orders = new List<OrderModel>();
                int totalRecords = 0;

                if (statusCode != (int)StatusResCode.Success)
                {
                    result.Code = (int)StatusResCode.Failed;
                    return Ok(result);
                }

                while (await reader.ReadAsync())
                {
                    OrderModel order = new OrderModel
                    {
                        OrderId = Convert.ToInt32(reader["OrderId"]),
                        OrderNumber = reader["OrderNumber"].ToString(),
                        OrderDate = Convert.ToDateTime(reader["OrderDate"]).ToString("yyyy-MM-dd"),
                        MemberName = reader["MemberName"].ToString(),
                        Receiver = reader["Receiver"].ToString(),
                        OrderAmount = Convert.ToDecimal(reader["OrderAmount"]),
                        OrderStatus = Convert.ToByte(reader["OrderStatus"]),
                        DeliveryStatus = Convert.ToByte(reader["DeliveryStatus"])
                    };
                    orders.Add(order);
                    totalRecords = Convert.ToInt32(reader["TotalRecords"]);
                }

                bool hasMore = (model.PageNumber * model.PageSize) < totalRecords;
                result.Code = (int)StatusResCode.Success;
                result.Data = orders;
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
        /// HTTP GET 取得訂單明細 API
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpGet, Route("api/orders/{orderId}")]
        public async Task<IHttpActionResult> GetOrderDetail([FromUri] GetOrderDetailRequest model)
        {
            var result = new GetOrderDetailResponseDto();

            if (model.OrderId == 0)
            {
                result.Code = (int)StatusResCode.InvalidFormat;
                return Ok(result);
            }

            bool checkPermission = (UserSession.Permission & (int)Permissions.ViewOrder) == (int)Permissions.ViewOrder;

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
                command = new SqlCommand("pro_bs_getOrderDetails", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@orderId", model.OrderId);
                SqlParameter statusCodeParam = new SqlParameter("@statusCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(statusCodeParam);
                reader = await command.ExecuteReaderAsync();
                OrderDetailResponse order = null;

                // 讀取訂單基本訊息
                if (await reader.ReadAsync())
                {
                    order = new OrderDetailResponse
                    {
                        OrderId = Convert.ToInt32(reader["OrderId"]),
                        OrderNumber = reader["OrderNumber"].ToString(),
                        OrderDate = Convert.ToDateTime(reader["OrderDate"]).ToString("yyyy-MM-dd"),
                        MemberName = reader["MemberName"].ToString(),
                        Receiver = reader["Receiver"].ToString(),
                        MobileNumber = reader["MobileNumber"].ToString(),
                        DeliveryAddress = reader["DeliveryAddress"].ToString(),
                        DeliveryMethod = Convert.ToByte(reader["DeliveryMethod"]),
                        DeliveryStatus = Convert.ToByte(reader["DeliveryStatus"]),
                        OrderAmount = Convert.ToDecimal(reader["OrderAmount"]),
                        OrderStatus = Convert.ToByte(reader["OrderStatus"]),
                        ReturnStatus = Convert.ToByte(reader["ReturnStatus"]),
                        SubtotalItems = new List<OrderDetailItemDto>()
                    };
                }

                // 讀取訂單明細訊息
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        OrderDetailItemDto orderDetail = new OrderDetailItemDto
                        {
                            OrderDetailId = Convert.ToInt32(reader["OrderDetailId"]),
                            ProductId = Convert.ToInt32(reader["ProductId"]),
                            ProductName = reader["ProductName"].ToString(),
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            Price = Convert.ToDecimal(reader["Price"]),
                            Subtotal = Convert.ToDecimal(reader["Subtotal"])
                        };
                        order.SubtotalItems.Add(orderDetail);
                    }
                }

                reader.Close();
                int statusCode = (int)statusCodeParam.Value;

                if (statusCode != (int)StatusResCode.Success)
                {
                    return StatusCode(HttpStatusCode.Unauthorized);
                }

                result.Code = (int)StatusResCode.Success;
                result.Data = order;
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
                }

                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        /// <summary>
        /// 更新訂單狀態
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/orders/updateOrderStatus")]
        public async Task<IHttpActionResult> UpdateOrderStatus(UpdateOrderStatusModel model)
        {
            var result = new UpdateOrderStatusResponseDto();

            bool checkPermission = (UserSession.Permission & (int)Permissions.EditOrder) == (int)Permissions.EditOrder;

            if (!checkPermission)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            if (model.OrderId == 0 || model.OrderStatus == 0)
            {
                result.Code = (int)StatusResCode.InvalidFormat;
                return Ok(result);
            }

            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                connection = new SqlConnection(SqlConfig.conStr);
                await connection.OpenAsync();
                command = new SqlCommand("pro_bs_editOrderStatus", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@orderId", model.OrderId);
                command.Parameters.AddWithValue("@orderStatus", model.OrderStatus);
                SqlParameter statusCodeParam = new SqlParameter("@statusCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(statusCodeParam);
                await command.ExecuteNonQueryAsync();
                int statusCode = (int)statusCodeParam.Value;

                if (statusCode != 0)
                {
                    result.Code = (int)StatusResCode.Failed;
                    return Ok(result);
                }
                else
                {
                    result.Code = (int)StatusResCode.Success;
                    return Ok(result);
                }
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
                }
            }
        }

        /// <summary>
        /// 更新訂單配送狀態
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/orders/updateDeliveryStatus")]
        public async Task<IHttpActionResult> UpdateDeliveryStatus(UpdateDeliveryStatusModel model)
        {
            var result = new UpdateDeliveryStatusResponseDto();

            bool checkPermission = (UserSession.Permission & (int)Permissions.EditOrder) == (int)Permissions.EditOrder;

            if (!checkPermission)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            if (model.OrderId == 0 || model.DeliveryStatus == 0)
            {
                result.Code = (int)StatusResCode.InvalidFormat;
                return Ok(result);
            }

            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                connection = new SqlConnection(SqlConfig.conStr);
                await connection.OpenAsync();
                command = new SqlCommand("pro_bs_editDeliveryStatus", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@orderId", model.OrderId);
                command.Parameters.AddWithValue("@deliveryStatus", model.DeliveryStatus);
                SqlParameter statusCodeParam = new SqlParameter("@statusCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(statusCodeParam);
                await command.ExecuteNonQueryAsync();
                int statusCode = (int)statusCodeParam.Value;

                if (statusCode != 0)
                {
                    result.Code = (int)StatusResCode.Failed;
                    return Ok(result);
                }
                else
                {
                    result.Code = (int)StatusResCode.Success;
                    return Ok(result);
                }
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
                }
            }
        }

        /// <summary>
        /// 更新訂單配送訊息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/orders/updateDeliveryMethod")]
        public async Task<IHttpActionResult> UpdateDeliveryMethod(UpdateDeliveryMethodModel model)
        {
            string mobilePattern = @"^09\d{8}$";
            string addresspPattern = @"^[\u4e00-\u9fa5]{2,3}[縣市][\u4e00-\u9fa5]{2,3}[鄉鎮市區].+$";
            var result = new UpdateDeliveryMethodResponseDto();

            bool checkPermission = (UserSession.Permission & (int)Permissions.EditOrder) == (int)Permissions.EditOrder;

            if (!checkPermission)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            if (model.OrderId == 0 ||  model.DeliveryMethod == 0)
            {
                result.Code = (int)StatusResCode.InvalidFormat;
                return Ok(result);
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(model.MobileNumber,mobilePattern) ||
                !System.Text.RegularExpressions.Regex.IsMatch(model.DeliveryAddress, addresspPattern))
            {
                result.Code = (int)StatusResCode.InvalidFormat;
                return Ok(result);
            }

            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                connection = new SqlConnection(SqlConfig.conStr);
                await connection.OpenAsync();
                command = new SqlCommand("pro_bs_editDeliveryMethod", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@orderId", model.OrderId);
                command.Parameters.AddWithValue("@deliveryMethod", model.DeliveryMethod);
                command.Parameters.AddWithValue("@deliveryAddress", model.DeliveryAddress ?? ((object)DBNull.Value));
                command.Parameters.AddWithValue("@receiver", model.Receiver);
                command.Parameters.AddWithValue("@mobileNumber", model.MobileNumber);
                SqlParameter statusCodeParam = new SqlParameter("@statusCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(statusCodeParam);
                await command.ExecuteNonQueryAsync();
                int statusCode = (int)statusCodeParam.Value;

                if (statusCode != 0)
                {
                    result.Code = (int)StatusResCode.Failed;
                    return Ok(result);
                }
                else
                {
                    result.Code = (int)StatusResCode.Success;
                    return Ok(result);
                }
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
                }
            }
        }

        /// <summary>
        /// 刪除訂單
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/orders/delete")]
        public async Task<IHttpActionResult> DeleteOrder(DeleteOrderModel model)
        {
            var result = new DeleteOrderResponseDto();

            bool checkPermission = (UserSession.Permission & (int)Permissions.DeleteOrder) == (int)Permissions.DeleteOrder;

            if (!checkPermission)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            if (model.OrderId == 0)
            {
                result.Code = (int)StatusResCode.InvalidFormat;
                return Ok(result);
            }

            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                connection = new SqlConnection(SqlConfig.conStr);
                await connection.OpenAsync();
                command = new SqlCommand("pro_bs_delOrder", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@orderId", model.OrderId);
                SqlParameter statusCodeParam = new SqlParameter("@statusCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(statusCodeParam);
                await command.ExecuteNonQueryAsync();
                int statusCode = (int)statusCodeParam.Value;

                if (statusCode != 0)
                {
                    result.Code = (int)StatusResCode.Failed;
                    return Ok(result);
                } else
                {
                    result.Code = (int)StatusResCode.Success;
                    return Ok(result);
                }
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
                }
            }
        }
    }
}