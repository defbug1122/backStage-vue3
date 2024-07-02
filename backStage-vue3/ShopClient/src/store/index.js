import Vue from "vue";

export const store = Vue.observable({
  // 當前用戶資料
  currentUser: {
    un: sessionStorage.getItem("currentUser"),
    role: sessionStorage.getItem("role"),
    token: sessionStorage.getItem("token"),
  },

  // 權限對應表
  permissionMap: {
    1: "新增帳號",
    2: "刪除帳號",
    4: "修改帳號",
    8: "查詢帳號",
    16: "查詢會員",
    32: "設置會員等級",
    64: "會員停權設定",
    128: "新增商品",
    256: "查看商品",
    512: "編輯商品",
    1024: "刪除商品",
    2048: "查看訂單",
    4096: "編輯訂單",
    8192: "刪除訂單",
  },
});

export const mutations = {
  setUserInfo(info) {
    store.currentUser.un = info.user;
    store.currentUser.role = info.role;
    store.currentUser.token = info.token;
  },
};
