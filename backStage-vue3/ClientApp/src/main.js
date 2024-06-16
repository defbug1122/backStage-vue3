import { createApp } from 'vue'
import './style.css'
import router from './router'
import App from './App.vue'
import axios from 'axios';

axios.interceptors.request.use((config) => {
    let token = sessionStorage.getItem('token');

    if (token)
        config.headers['Authorization'] = `Bearer ${token}`
    return config;
}, (error) => {
    return Promise.reject(error);
});

axios.interceptors.response.use(
    (response) => {
        // 如果HTTP狀態碼是200，則直接返回響應
        return response;
    },
    (error) => {
        if (error.response.status === 401) {
            sessionStorage.removeItem("token");
            sessionStorage.removeItem("role");
            alert('重新登入')
            router.push('/login');
            console.log('重新登入')
        } else if (error.response.status === 500) {
            alert('目前維修中，請聯繫系統管理員')
        }        
        else {
            console.log('操作失敗')
    }
});

createApp(App).use(router).mount('#app')