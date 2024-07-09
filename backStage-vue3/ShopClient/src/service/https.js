import axios from "axios";
import router from "../router"; // 根據你的項目結構調整路徑

// 創建一個 axios 實例
const instance = axios.create({
  timeout: 10000,
});

// 添加請求攔截器
instance.interceptors.request.use(
  (config) => {
    let token = sessionStorage.getItem("token");
    if (token) {
      config.headers["Authorization"] = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// 添加響應攔截器
instance.interceptors.response.use(
  (response) => {
    // 如果HTTP狀態碼是200，則直接返回響應
    return response;
  },
  (error) => {
    if (error.response.status === 401) {
      if (error.response.data.code === 5) {
        sessionStorage.removeItem("token");
        sessionStorage.removeItem("role");
        alert("此帳號已在其他地方登入，請重新登入");
        router.push("/login");
      } else if (error.response.data.code === 13) {
        sessionStorage.removeItem("token");
        sessionStorage.removeItem("role");
        alert("身分驗證失敗，重新登入");
        router.push("/login");
      } else if (error.response.data.code === 12) {
        sessionStorage.removeItem("token");
        sessionStorage.removeItem("role");
        alert("權限被更改，重新登入");
        router.push("/login");
      }
    } else if (error.response.status === 403) {
      router.push("/");
      alert("權限不足，無法操作");
      console.log("權限不足，無法操作");
    } else if (error.response.status === 404) {
      router.push("/notFound");
      console.log("找不到頁面");
    } else if (error.response.status === 500) {
      router.push("/");
      alert("目前維修中，請聯繫系統管理員");
      console.log("請稍後再試");
    } else {
      console.log("操作失敗");
    }
    return Promise.reject(error);
  }
);

export default instance;
