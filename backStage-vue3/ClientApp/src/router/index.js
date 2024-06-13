import { createRouter, createWebHashHistory } from 'vue-router'
import Home from '../components/Home.vue'
import Login from '../components/Login.vue'
import Account from '../components/Account.vue'
import Member from '../components/Member.vue'
import Order from '../components/Order.vue'
import Product from '../components/Product.vue'

const routes = [
  {
    path: '/',
    name: 'Home',
    component: Home,
    meta: { requiresAuth: true }
  },
  {
    path: '/login',
    name: 'Login',
    component: Login
  },
  {
    path: '/account',
    name: 'Account',
    component: Account
  },
  {
    path: '/member',
    name: 'Member',
    component: Member
  },
  {
    path: '/order',
    name: 'Order',
    component: Order
  },
  {
    path: '/product',
    name: 'Product',
    component: Product
  },
]


const router = createRouter({
    history: createWebHashHistory(),
    routes
});

router.beforeEach((to, from, next) => {
  const token = sessionStorage.getItem('token');
  if (to.path === '/login' && token) {
    next({path: '/'});
  } else if (to.path === '/' && !token) {
    next({path: '/login'});
  } else {
    next();
  }
});


export default router