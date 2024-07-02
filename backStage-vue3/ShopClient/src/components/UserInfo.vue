<template>
  <div class="user-info">
    <div>當前用戶: {{ user.un }}</div>
    <el-button type="primary" @click="logout">登出</el-button>
  </div>
</template>

<script>
import { logout } from "@/service/api";
import { store, mutations } from "@/store";

export default {
  name: "UserInfo",
  props: {},
  data() {
    return {
      user: {
        un: store.currentUser.un,
      },
    };
  },
  methods: {
    // 呼叫登出 API
    async logout() {
      try {
        const response = await logout();
        if (response.data.code === 0) {
          this.$message({
            message: "登出成功",
            type: "success",
            duration: 1200,
          });
          // 清理相關資料
          sessionStorage.removeItem("token");
          sessionStorage.removeItem("currentUser");
          sessionStorage.removeItem("role");
          mutations.setUserInfo({
            user: "",
            role: "",
            token: "",
          });
          this.$router.push("/login");
        } else {
          this.$message({
            message: "登出失败",
            type: "error",
            duration: 1200,
          });
        }
      } catch (error) {
        this.$message({
          message: "登出请求失败",
          type: "error",
          duration: 1200,
        });
      }
    },
  },
};
</script>

<style scoped>
.user-info {
  display: flex;
  justify-content: end;
  align-items: center;
}
</style>
