import https from "./https";

// 登入API
export const Login = (params) => {
  return https.post("api/user/login", params);
};

// 登出API
export const Logout = () => {
  return https.post("api/user/logout");
};

// 取得用戶列表API
export const GetUserList = (params) => {
  return https.get(
    `/api/user/list?searchTerm=${params.searchTerm}&pageNumber=${params.pageNumber}&pageSize=${params.pageSize}&sortBy=${params.sortBy}`
  );
};

// 創造用戶API
export const CreateAcc = (params) => {
  return https.post("api/user/add", params);
};

// 刪除用戶API
export const DeleteAcc = (params) => {
  return https.post("api/user/delete", params);
};

// 更新用戶權限API
export const EditAccRole = (params) => {
  return https.post("api/user/updateRole", params);
};

// 更新用戶資訊API
export const EditAccPwd = (params) => {
  return https.post("api/user/editInfo", params);
};

// 取得會員列表API
export const GetMemberList = (params) => {
  return https.get(
    `/api/member/list?searchTerm=${params.searchTerm}&pageNumber=${params.pageNumber}&pageSize=${params.pageSize}&sortBy=${params.sortBy}`
  );
};

// 更新會員狀態API
export const UpdateMemberStatus = (params) => {
  return https.post("api/member/updateStatus", params);
};

// 更新會員等級API
export const UpdateMemberLevel = (params) => {
  return https.post("api/member/updateLevel", params);
};

// 取得商品列表API
export const GetProductList = (params) => {
  return https.get(
    `/api/product/list?searchTerm=${params.searchTerm}&pageNumber=${params.pageNumber}&pageSize=${params.pageSize}&sortBy=${params.sortBy}`
  );
};

// 新增商品API
export const CreateProduct = (params) => {
  return https.post("api/product/add", params);
};

// 編輯商品API
export const EditProduct = (params) => {
  return https.post("api/product/edit", params);
};

// 刪除商品API
export const DeleteProduct = (params) => {
  return https.post("api/product/delete", params);
};

// 獲取訂單列表
export function GetOrderList(params) {
  return https.get(`/api/orders/list`, { params });
}

// 獲取訂單详情
export function GetOrderDetails(params) {
  return https.get(`/api/orders/${params.orderId}`);
}

// 修改訂單配送方式
export function UpdateDeliveryMethod(params) {
  return https.post("/api/orders/updateDeliveryMethod", params);
}

// 刪除訂單
export function DeleteOrder(params) {
  return https.post("/api/orders/delete", params);
}
