import https from "./https";

// 登入API
export const login = (params) => {
  return https.post("api/user/login", params);
};

// 登出API
export const logout = () => {
  return https.post("api/user/logout");
};

// 取得用戶列表API
export const getUserList = (params) => {
  return https.get(
    `/api/user/list?searchTerm=${params.searchTerm}&pageNumber=${params.pageNumber}&pageSize=${params.pageSize}&sortBy=${params.sortBy}`
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

// 取得會員列表API
export const getMemberList = (params) => {
  return https.get(
    `/api/member/list?searchTerm=${params.searchTerm}&pageNumber=${params.pageNumber}&pageSize=${params.pageSize}&sortBy=${params.sortBy}`
  );
};

// 更新會員狀態API
export const updateMemberStatus = (params) => {
  return https.post("api/member/updateStatus", params);
};

// 更新會員等級API
export const updateMemberLevel = (params) => {
  return https.post("api/member/updateLevel", params);
};
