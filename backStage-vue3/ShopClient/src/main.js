import Vue from "vue";
import App from "./App.vue";
import VueRouter from "vue-router";
import router from "./router";
import axios from "./service/https";
import VueAxios from "vue-axios";
import "./style.css";
import ElementUI from "element-ui";
import "element-ui/lib/theme-chalk/index.css";

Vue.use(VueRouter);
Vue.use(VueAxios, axios);
Vue.use(ElementUI);

const routerPush = VueRouter.prototype.push;

VueRouter.prototype.push = function push(location, onResolve, onReject) {
  if (onResolve || onReject)
    return routerPush.call(this, location, onResolve, onReject);
  return routerPush.call(this, location).catch((error) => error);
};

new Vue({
  router,
  render: (h) => h(App),
}).$mount("#app");
