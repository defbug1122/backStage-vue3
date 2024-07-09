<template>
  <div class="user-info">
    <span>當前用戶: {{ user.userName }}</span>
    <el-button type="info" @click="OpenEditPasswordModal">編輯</el-button>
    <el-button type="primary" class="logout-button" @click="Logout"
      >登出</el-button
    >

    <!-- 編輯密碼彈窗 -->
    <EditPwdModal
      :showModal="showEditPasswordModal"
      :user="user"
      @close="CloseEditPasswordModal"
      @save="HandleEditPasswordSave"
    />
  </div>
</template>

<script>
import { Logout, EditAccPwd } from "@/service/api";
import { store, mutations } from "@/store";
import EditPwdModal from "@/components/Modal/EditPwdModal.vue";

export default {
  name: "UserInfo",
  components: {
    EditPwdModal,
  },
  data() {
    return {
      user: {
        id: store.currentUser.id,
        userName: store.currentUser.user,
        pwd: "",
      },
      showEditPasswordModal: false,
    };
  },
  methods: {
    // 打開編輯密碼彈窗
    OpenEditPasswordModal() {
      this.showEditPasswordModal = true;
    },

    // 關閉編輯密碼彈窗
    CloseEditPasswordModal() {
      this.showEditPasswordModal = false;
    },

    // 保存編輯密碼
    HandleEditPasswordSave(user) {
      // 調用更新密碼的邏輯
      this.UpdatePassword(user);
    },

    // 呼叫更新密碼 API
    async UpdatePassword(user) {
      if (!user.pwd) {
        this.$message({
          message: "密碼為空",
          type: "error",
          duration: 1200,
        });
        return;
      }
      const pattern = /^[a-zA-Z0-9_-]{4,16}$/;
      if (!pattern.test(user.pwd)) {
        this.$message({
          message: "密碼必須是4-16個字符，只能包含字母、數字、下劃線和連字符",
          type: "error",
          duration: 1200,
        });
        return;
      }
      try {
        const response = await EditAccPwd(user);
        if (response.data.code === 0) {
          this.$message({
            message: "密碼更新成功",
            type: "success",
            duration: 1200,
          });
          this.CloseEditPasswordModal();
        } else {
          this.$message({
            message: "密碼更新失敗",
            type: "error",
            duration: 1200,
          });
        }
      } catch (error) {
        console.error("error", error);
        this.$message({
          message: "密碼更新失敗",
          type: "error",
          duration: 1200,
        });
      }
    },

    // 呼叫登出 API
    async Logout() {
      try {
        const response = await Logout();
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
          mutations.SetUserInfo({
            id: "",
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
  margin: 10px;
}

.logout-button {
  background-color: #333333;
  border-color: unset;
}

.logout-button:hover {
  background-color: #000000;
  border-color: unset;
}

span {
  margin-right: 20px;
}
</style>
