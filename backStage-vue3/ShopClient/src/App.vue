<template>
  <el-container>
    <el-aside width="180px" v-if="token">
      <Menu />
    </el-aside>
    <el-main>
      <router-view />
    </el-main>
  </el-container>
</template>

<script>
import Menu from "./components/Menu.vue";

export default {
  name: "App",
  components: {
    Menu,
  },
  data() {
    return {
      token: null,
      currentUser: null,
    };
  },
  created() {
    this.token = sessionStorage.getItem("token");
    this.currentUser = sessionStorage.getItem("currentUser");
    if (!this.token) {
      this.$router.push("/login");
    }
  },
  watch: {
    $route() {
      this.token = sessionStorage.getItem("token");
      this.currentUser = sessionStorage.getItem("currentUser");
    },
  },
};
</script>

<style scoped>
.el-container {
  height: 100vh;
}
.user-block {
  text-align: right;
}
.el-main {
  padding: 0;
}
</style>
