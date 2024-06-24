import Vue from "vue";

export const store = Vue.observable({
  currentUser: {
    un: sessionStorage.getItem("currentUser"),
    role: sessionStorage.getItem("role"),
    token: sessionStorage.getItem("token"),
  },
});

export const mutations = {
  setUserInfo(info) {
    store.currentUser.un = info.user;
    store.currentUser.role = info.role;
    store.currentUser.token = info.token;
  },
};
