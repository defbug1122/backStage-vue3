import Vue from "vue";
import Router from "vue-router";
import Home from "@/views/Home.vue";
import Login from "@/views/Login.vue";
import Account from "@/views/Account.vue";
import Member from "@/views/Member.vue";
import Order from "@/views/Order.vue";
import Product from "@/views/Product.vue";
import NotFound from "@/views/NotFound.vue";

Vue.use(Router);

const routes = [
  {
    path: "/",
    name: "Home",
    component: Home,
  },
  {
    path: "/login",
    name: "Login",
    component: Login,
  },
  {
    path: "/account",
    name: "Account",
    component: Account,
  },
  {
    path: "/member",
    name: "Member",
    component: Member,
  },
  {
    path: "/order",
    name: "Order",
    component: Order,
  },
  {
    path: "/product",
    name: "Product",
    component: Product,
  },
  {
    path: "/notFound",
    name: "NotFound",
    component: NotFound,
  },
  {
    path: "*",
    redirect: "/",
  },
];

const router = new Router({
  routes,
});

router.beforeEach((to, from, next) => {
  const token = sessionStorage.getItem("token");
  if (to.path === "/login" && token) {
    next({ path: "/" });
  } else if (to.path === "/" && !token) {
    next({ path: "/login" });
  } else {
    next();
  }
});

export default router;
