import { createRouter, createWebHashHistory } from 'vue-router'
import Home from '../components/Home.vue'
import Login from '../components/Login.vue'

const routes = [
  {
    path: '/',
    name: 'Home',
    component: Home
  },
  {
    path: '/login',
    name: 'Login',
    component: Login
  },
]


const router = createRouter({
    history: createWebHashHistory(),
    routes
});

router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('token');
  const userRole = localStorage.getItem('role');

  // const hasAuthToRouter = to.matched.some(record => record.meta.requiresAuth)
  // const hasMatchRouter = to.matched.some(record => (record.meta.roles || []).find(role=>role === userRole))

  // if(hasAuthToRouter && token){
  //   console.log('eeeeeeeeeeffffffff');
  //     //需要驗證才能進的路由
  //     if(hasMatchRouter){
  //      //匹配權限的路由
  //         next()
  //     } else {
  //      //不匹配權限的路由
  //         if (userRole === "SUPPLIER") {
  //             next({path: '/orderManagement'});
  //         } else if (userRole === "BRAND_LEADER" || userRole === "GROUP_LEADER" || userRole === "ADMIN" || userRole === "STORE_LEADER") {
  //             next({path: '/basicSettings/userAccount'});
  //         } else {
  //             next({path: '/'});
  //         }
  //     }
  // } else {
  //     //不用驗證權限
  //     if (to.path === '/login' && token) {
  //         next({path: '/'});
  //     } else if (to.path === '/' && !token) {
  //         next({path: '/login'});
  //     } else {
  //         next();
  //     }
  // }

  if (to.matched.some(record => record.meta.requiresAuth)) {
      if (!token) {
          next({path: '/login'});
      } else {
          next();
      }
  } else {
      if (to.path === '/login' && token) {
          next({path: '/'});
      } else if (to.path === '/' && !token) {
          next({path: '/login'});
      } else {
          next();
      }
  }
})


export default router