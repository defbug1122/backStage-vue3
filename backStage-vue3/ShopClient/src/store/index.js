import Vue from "vue";

export const store = Vue.observable({
  // 當前用戶資料
  currentUser: {
    id: sessionStorage.getItem("id"),
    user: sessionStorage.getItem("currentUser"),
    role: Number(sessionStorage.getItem("role")),
    token: sessionStorage.getItem("token"),
  },
});

export const mutations = {
  SetUserInfo(info) {
    store.currentUser.id = Number(info.id);
    store.currentUser.user = info.user;
    store.currentUser.role = Number(info.role);
    store.currentUser.token = info.token;
  },
};
