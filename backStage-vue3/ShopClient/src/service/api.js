import https from "./https";

// 登入API
export const login = (params) => {
  return https.post("api/user/login", params);
};

// 取得用戶列表API
export const getUserList = (params) => {
  return https.get(
    `/api/user/list?searchTerm=${params.searchTerm}&pageNumber=${params.pageNumber}&pageSize=${params.pageSize}`
  );
};

// 創造用戶API
export const createAcc = (params) => {
  return https.post("api/user/add", params);
};

// 刪除用戶API
export const deleteAcc = (params) => {
  return https.post("api/user/delete", params);
};

// 更新用戶資訊API
export const editAcc = (params) => {
  return https.post("api/user/update", params);
};
